using System;
using System.IO;
using System.Threading.Tasks;
using Minecraft.Extensions;
using Minecraft.Protocol.Data;
using Minecraft.Protocol.Packets;
using Minecraft.Protocol.Packets.Client;
using Minecraft.Protocol.Packets.Server;

namespace Minecraft.Protocol
{
    /// <summary>
    ///     协议适配器
    /// </summary>
    public class ProtocolAdapter
    {
        private readonly Stream _baseStream;
        private readonly BufferedStream _receiveStream;
        private readonly BufferedStream _sendStream;

        /// <summary>
        ///     创建协议适配器
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="boundTo">绑定至</param>
        public ProtocolAdapter(Stream stream, PacketBoundTo boundTo)
        {
            _baseStream = stream;
            _receiveStream = new BufferedStream(stream);
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
        public bool Compressing { get; private set; } = false;

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

            await Task.Yield();
            var content = new ByteArray(0);
            packet.WriteToStream(content);
            HandleSpecialPacket(packet);
            content.Position = 0;
            lock (_sendStream)
            {
                var dataPacket = new DataPacket(packet.PacketId, packet.BoundTo, content, State);
                if (Compressing) dataPacket.WriteCompressedToStream(_sendStream);
                else dataPacket.WriteToStream(_sendStream);
                _sendStream.Flush();
            }

            if (packet is KeepAliveResponsePacket)
                _ = Logger.Debug<ProtocolAdapter>("keep alive -> S");

            _ = Task.Run(() =>
            {
                packet.OnSend(this);
                PacketSent?.Invoke(this, packet);
            }).LogException<ProtocolAdapter>();
        }

        private void HandleSpecialPacket(Packet packet)
        {
            // _ = Logger.Debug<ProtocolAdapter>($"received packet: 0x{packet.PacketId.ToString("X").PadLeft(2, '0')} {packet.GetType().Name}");
            switch (packet)
            {
                case LoginSetCompressionPacket loginSetCompressionPacket:
                    Compressing = loginSetCompressionPacket.Threshold > 0;
                    _ = Logger.Info<ProtocolAdapter>($"Set compression: {Compressing}");
                    break;
                case HandshakePacket handshakePacket:
                    State = handshakePacket.NextState;
                    _ = Logger.Debug<ProtocolAdapter>($"Change protocol state: {State}");
                    break;
                case LoginSuccessPacket _:
                    State = ProtocolState.Play;
                    _ = Logger.Debug<ProtocolAdapter>($"Change protocol state: {State}");
                    break;
            }
        }

        /// <summary>
        ///     启动适配器
        /// </summary>
        public async Task Start()
        {
            await Task.Run(() =>
            {
                Logger.SetExceptionHandler();
                Logger.SetThreadName("ProtocolAdapterThread");
                if (Running) throw new ProtocolException("Adapter has already running");
                Running = true;
                _ = Logger.Info<ProtocolAdapter>("Adapter started");
                State = ProtocolState.Handshaking;

                try
                {
                    HandleTask();
                }
                catch (Exception ex)
                {
                    _ = Logger.Warn<ProtocolAdapter>("Adapter stoped with exception.");
                    _ = Task.Run(() => Exception?.Invoke(this, ex)).LogException<ProtocolAdapter>();
                }
                finally
                {
                    Running = false;
                }
            });
        }

        private bool _stopping = false;

        /// <summary>
        ///     停止适配器
        /// </summary>
        public async Task Stop()
        {
            if (_stopping)
            {
                await Task.Yield();
                lock (_baseStream)
                {
                }
                return;
            }
            _stopping = true;
            await Task.Yield();
            lock (_baseStream)
            {
                _baseStream.Close();
            }
            _stopping = false;
        }

        private void HandleTask()
        {
            _ = Task.Run(() => Started?.Invoke(this, EventArgs.Empty)).LogException<ProtocolAdapter>();
            while (true)
            {
                Packet packet;
                try
                {
                    packet = ReceivePacket();
                }
                catch (PacketParseException ex)
                {
                    //_ = Logger.Warn<ProtocolAdapter>(ex.Message);
                    // ignore
                    continue;
                }
                catch (ProtocolException ex)
                {
                    _ = Logger.Warn<ProtocolAdapter>(ex.Message);
                    continue;
                }
                catch (EndOfStreamException)
                {
                    _ = Task.Run(() => Stopped?.Invoke(this, EventArgs.Empty)).LogException<ProtocolAdapter>();
                    _ = Logger.Info<ProtocolAdapter>("Adapter has been stopped. EOF");
#if DEBUG
                    throw;
#else
                    return;
#endif
                }
                catch
                {
                    _ = Task.Run(() => Stopped?.Invoke(this, EventArgs.Empty)).LogException<ProtocolAdapter>();
                    if (!_stopping)
                    {
                        throw;
                    }
                    _ = Logger.Info<ProtocolAdapter>("Adapter has been stopped.");
                    return;
                }

                HandleSpecialPacket(packet);
                HandleKeepAlivePacket(packet);

                _ = Task.Run(() =>
                {
                    PacketReceived?.Invoke(this, packet);
                }).LogException<ProtocolAdapter>();
            }
        }

        private void HandleKeepAlivePacket(Packet packet)
        {
            if (packet is KeepAlivePacket keepAlivePacket)
            {
                _ = Logger.Debug<ProtocolAdapter>($"S -> keep alive {keepAlivePacket.KeepAliveId}");
                _ = SendPacket(new KeepAliveResponsePacket { KeepAliveId = keepAlivePacket.KeepAliveId }).HandleException(ex => Exception?.Invoke(this, ex)).LogException<ProtocolAdapter>();
            }
        }

        private Packet ReceivePacket()
        {
            lock (_receiveStream)
                return Packet.ReadPacket(_receiveStream, RemoteBoundTo, () => State, () => Compressing);
        }

        public async Task<Packet> ReceiveSinglePacket()
        {
            var taskCompletionSource = new TaskCompletionSource<Packet>();
            void StoppedHandler(object sender, EventArgs e)
            {
                taskCompletionSource.SetCanceled();

                Stopped -= StoppedHandler;
                PacketReceived -= Handler;
            }
            Stopped += StoppedHandler;
            void Handler(object sender, Packet packet)
            {
                taskCompletionSource.SetResult(packet);

                Stopped -= StoppedHandler;
                PacketReceived -= Handler;
            }
            PacketReceived += Handler;
            return await taskCompletionSource.Task;
        }

        public async Task<T> ReceiveSinglePacket<T>() where T : Packet
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            PacketHandleResult handler = null;
            void StoppedHandler(object sender, EventArgs e)
            {
                taskCompletionSource.TrySetCanceled();
            }
            void Handler(T packet)
            {
                taskCompletionSource.TrySetResult(packet);
            }
            handler = HandlePacket<T>(Handler);
            Stopped += StoppedHandler;
            try
            {
                var result = await taskCompletionSource.Task;
                return result;
            }
            catch (TaskCanceledException)
            {
                throw new ProtocolException("Protocol stopped.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                UnHandlePacket(handler);
                Stopped -= StoppedHandler;
            }
        }

        public async Task HandleReceiveSinglePacket(Action<Packet> handler)
        {
            var packet = await ReceiveSinglePacket();
            handler(packet);
        }

        public async Task HandleReceiveSinglePacket<T>(Action<T> handler) where T : Packet
        {
            var packet = await ReceiveSinglePacket<T>();
            handler(packet);
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
        public event EventHandler<Exception> Exception;

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