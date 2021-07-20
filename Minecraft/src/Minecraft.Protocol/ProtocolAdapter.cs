using System;
using System.IO;
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
        private readonly Stream _receiveStream;
        private readonly BufferedStream _sendStream;

        /// <summary>
        ///     创建协议适配器
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="boundTo">绑定至</param>
        public ProtocolAdapter(Stream stream, PacketBoundTo boundTo)
        {
            _receiveStream = stream;
            _sendStream = new BufferedStream(stream);
            BoundTo = boundTo;
        }

        /// <summary>
        ///     写入到流的数据包绑定
        /// </summary>
        public PacketBoundTo BoundTo { get; }

        /// <summary>
        /// 从流读出的数据包绑定
        /// </summary>
        public PacketBoundTo RemoteBoundTo
        {
            get
            {
                return BoundTo switch
                {
                    PacketBoundTo.Client => PacketBoundTo.Server,
                    PacketBoundTo.Server => PacketBoundTo.Client,
                    _ => default
                };
            }
        }

        /// <summary>
        ///     当前的协议状态
        /// </summary>
        public ProtocolState State { get; private set; } = ProtocolState.Any;

        /// <summary>
        ///     启用压缩
        /// </summary>
        public bool Compressing { get; private set; }

        /// <summary>
        ///     适配器是否在运行
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        ///     发送数据包
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        /// <exception cref="ProtocolException">包类型与写入流的Bound不匹配</exception>
        public async Task SendPacket(Packet packet)
        {
            if (packet.BoundTo != BoundTo)
                throw new ProtocolException("packet boundTo incorrect");
            if (packet.State != State && State != ProtocolState.Any)
                throw new ProtocolException($"packet state incorrect, current state: {State}");
            await Task.Run(() =>
            {
                var content = new ByteArray(0);
                packet.WriteToStream(content);
                content.Position = 0;
                lock (_sendStream)
                {
                    new DataPacket(packet.PacketId, packet.BoundTo, content, State).WriteToStream(_sendStream);
                    _sendStream.Flush();
                }

                Task.Run(() =>
                {
                    PrivatePacketSent?.Invoke(this, packet);
                    PacketSent?.Invoke(this, packet);
                });
            });
        }

        private event EventHandler<Packet> PrivatePacketSent;
        private event EventHandler<Packet> PrivatePacketReceived;

        /// <summary>
        ///     启动适配器
        /// </summary>
        public async Task Run()
        {
            await Task.Run(async () =>
            {
                Logger.SetExceptionHandler();
                Logger.SetThreadName("ProtocolAdapterThread");
                if (Running) throw new ProtocolException("Adapter has already running");
                Running = true;
                Logger.Info<ProtocolAdapter>("Adapter started");
                State = ProtocolState.Handshaking;

                void HandleHandshake(object sender, Packet e)
                {
                    if (e is HandshakePacket handshakePacket)
                        State = handshakePacket.NextState;
                }

                PrivatePacketReceived += HandleHandshake;
                PrivatePacketSent += HandleHandshake;

                var handleTask = Task.Run(HandleTask);
                await handleTask;
                handleTask.Exception?.Handle(exception =>
                {
                    Logger.Fatal<ProtocolAdapter>(exception);
                    return true;
                });
                Running = false;
            });
        }

        /// <summary>
        ///     停止适配器
        /// </summary>
        public async void Stop()
        {
            await Task.Run(() =>
            {
                lock (_receiveStream)
                {
                    _receiveStream.Close();
                }
            });
        }

        private void HandleTask()
        {
            Task.Run(() => Started?.Invoke(this, EventArgs.Empty));
            while (true)
            {
                Packet packet;
                try
                {
                    packet = ReceivePacket();
                }
                catch (EndOfStreamException)
                {
                    Task.Run(() => Stopped?.Invoke(this, EventArgs.Empty));
                    Logger.Info<ProtocolAdapter>("Adapter has been stopped.");
                    return;
                }

                Task.Run(() =>
                {
                    PrivatePacketReceived?.Invoke(this, packet);
                    PacketReceived?.Invoke(this, packet);
                });
            }
        }

        private Packet ReceivePacket()
        {
            lock (_receiveStream)
                return Packet.ReadPacket(_receiveStream, RemoteBoundTo, State, Compressing);
        }

        /// <summary>
        ///     接收到数据包
        /// </summary>
        public event EventHandler<Packet> PacketReceived;

        /// <summary>
        ///     发送了数据包
        /// </summary>
        public event EventHandler<Packet> PacketSent;

        public event EventHandler Started;
        public event EventHandler Stopped;

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