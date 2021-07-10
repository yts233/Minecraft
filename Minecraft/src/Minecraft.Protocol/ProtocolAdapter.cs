using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Minecraft.Protocol.Data;
using Minecraft.Protocol.Packets;
using Minecraft.Protocol.Packets.Client;

namespace Minecraft.Protocol
{
    /// <summary>
    ///     协议适配器
    /// </summary>
    public class ProtocolAdapter
    {
        private readonly IPEndPoint _remoteEndPoint;
        private Stream _receiveStream;
        private BufferedStream _sendStream;

        /// <summary>
        ///     创建协议适配器
        /// </summary>
        /// <param name="remoteEndPoint">远程地址</param>
        /// <param name="streamOrigin">远程类型</param>
        public ProtocolAdapter(IPEndPoint remoteEndPoint, PacketOrigin streamOrigin)
        {
            _remoteEndPoint = remoteEndPoint;
            ReceiveOrigin = streamOrigin;
        }

        /// <summary>
        ///     创建协议适配器
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="origin">流类型</param>
        public ProtocolAdapter(Stream stream, PacketOrigin origin)
        {
            _receiveStream = stream;
            _sendStream = new BufferedStream(stream);
            ReceiveOrigin = origin;
        }

        /// <summary>
        ///     从流读取数据包的类型
        /// </summary>
        public PacketOrigin ReceiveOrigin { get; }

        /// <summary>
        ///     写入到流的数据包类型
        /// </summary>
        public PacketOrigin SendOrigin => ReceiveOrigin switch
        {
            PacketOrigin.Client => PacketOrigin.Server,
            PacketOrigin.Server => PacketOrigin.Client,
            _ => PacketOrigin.Client
        };

        /// <summary>
        ///     当前的协议状态
        /// </summary>
        public ProtocolState State { get; set; } = ProtocolState.Any;

        /// <summary>
        ///     启用压缩
        /// </summary>
        public bool Compressing { get; } = false;

        /// <summary>
        ///     适配器是否在运行
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        ///     清除缓冲区，立即把数据包写入到流
        /// </summary>
        public void Flush()
        {
            _sendStream.Flush();
        }

        /// <summary>
        ///     异步清除缓冲区
        /// </summary>
        /// <returns></returns>
        public async Task FlushAsync()
        {
            await _sendStream.FlushAsync();
        }

        /// <summary>
        ///     发送数据包，别忘了用 <see cref="Flush" /> 或 <see cref="FlushAsync" /> 清除缓冲区
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        /// <exception cref="ProtocolException">包类型与写入流的类型不匹配</exception>
        public ProtocolAdapter SendPacket(Packet packet)
        {
            if (packet.Origin != SendOrigin)
                throw new ProtocolException("packet origin incorrect");
            var content = new ByteArray(0);
            packet.WriteToStream(content);
            content.Position = 0;
            new DataPacket(packet.PacketId, packet.Origin, content, State).WriteToStream(_sendStream);
            PacketSent?.Invoke(this, packet);
            return this;
        }

        /// <summary>
        ///     启动适配器
        /// </summary>
        public async Task Run()
        {
            Logger.SetExceptionHandler();
            if (Running) throw new ProtocolException("Adapter has already running");
            Running = true;
            if (_remoteEndPoint != null)
            {
                var client = new TcpClient();
                client.Connect(_remoteEndPoint);
                _receiveStream = client.GetStream();
                _sendStream = new BufferedStream(_receiveStream);
            }

            Logger.Info<ProtocolAdapter>("Adapter started");
            State = ProtocolState.Handshaking;

            void HandleHandshake(HandshakePacket packet)
            {
                State = packet.NextState;
            }

            HandlePacket((Action<HandshakePacket>) HandleHandshake);
            HandleSendPacket((Action<HandshakePacket>) HandleHandshake);
            var handleTask = Task.Run(HandleTask);
            await handleTask;
            handleTask.Exception?.Handle(exception =>
            {
                Logger.Fatal<ProtocolAdapter>(exception.ToString());
                return true;
            });
            Running = false;
        }

        /// <summary>
        ///     停止适配器
        /// </summary>
        public void Stop()
        {
            _receiveStream.Close();
        }

        private void HandleTask()
        {
            while (true)
            {
                Packet packet;
                try
                {
                    packet = ReceivePacket();
                }
                catch (EndOfStreamException)
                {
                    Logger.Info<ProtocolAdapter>("Adapter has been stopped.");
                    return;
                }

                new Task(() => PacketReceived?.Invoke(this, packet)).Start();
            }
        }

        private Packet ReceivePacket()
        {
            return Packet.ReadPacket(_receiveStream, ReceiveOrigin, State, Compressing);
        }

        /// <summary>
        ///     接收到数据包
        /// </summary>
        public event EventHandler<Packet> PacketReceived;

        /// <summary>
        ///     发送了数据包
        /// </summary>
        public event EventHandler<Packet> PacketSent;

        /// <summary>
        ///     处理指定数据包
        /// </summary>
        /// <param name="callback">委托</param>
        /// <typeparam name="T">数据包的类型</typeparam>
        /// <returns></returns>
        public PacketHandleResult HandlePacket<T>(Action<T> callback) where T : Packet
        {
            void EventHandler(object sender, Packet e)
            {
                if (e is T packet) callback(packet);
            }

            PacketReceived += EventHandler;
            return new PacketHandleResult(EventHandler);
        }

        /// <summary>
        ///     取消处理指定数据包
        /// </summary>
        /// <param name="result">处理数据包返回的结果</param>
        public void UnHandlePacket(PacketHandleResult result)
        {
            PacketReceived -= result.Handler;
        }

        /// <summary>
        ///     处理发送了某个数据包
        /// </summary>
        /// <param name="callback">委托</param>
        /// <typeparam name="T">数据包类型</typeparam>
        /// <returns></returns>
        public PacketHandleResult HandleSendPacket<T>(Action<T> callback) where T : Packet
        {
            void EventHandler(object sender, Packet e)
            {
                if (e is T packet) callback(packet);
            }

            PacketSent += EventHandler;
            return new PacketHandleResult(EventHandler);
        }

        /// <summary>
        ///     取消处理发送了某个数据包
        /// </summary>
        /// <param name="result">处理发送了某个数据包返回的结果</param>
        public void UnHandleSendPacket(PacketHandleResult result)
        {
            PacketSent -= result.Handler;
        }

        /// <summary>
        ///     数据包处理返回的结果
        /// </summary>
        public class PacketHandleResult
        {
            internal readonly EventHandler<Packet> Handler;

            internal PacketHandleResult(EventHandler<Packet> handler)
            {
                Handler = handler;
            }
        }
    }
}