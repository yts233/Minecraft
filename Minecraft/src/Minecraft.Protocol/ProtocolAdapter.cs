using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Minecraft.Extensions;
using Minecraft.Protocol.Data;
using Minecraft.Protocol.Packets;
using Minecraft.Protocol.Packets.Client;
using Minecraft.Protocol.Packets.Server;

namespace Minecraft.Protocol
{
    /// <summary>
    /// 协议适配器
    /// </summary>
    public class ProtocolAdapter
    {
        private static readonly Logger<ProtocolAdapter> _logger = Logger.GetLogger<ProtocolAdapter>();
        private readonly Stream _baseStream;
        private readonly BufferedStream _receiveStream;
        private readonly BufferedStream _sendStream;
        private readonly object _runningLock = new object();

        private readonly Queue<Packet> _receiveQueue = new Queue<Packet>();
        private readonly Queue<Packet> _sendQueue = new Queue<Packet>();
        private Exception _lastSendException;
        private readonly object _receiveLock = new object();

        /// <summary>
        /// 创建协议适配器
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="boundTo">写入流绑定至</param>
        public ProtocolAdapter(Stream stream, PacketBoundTo boundTo)
        {
            _baseStream = stream;
            _receiveStream = new BufferedStream(stream);
            _sendStream = new BufferedStream(stream);
            BoundTo = boundTo;
        }

        /// <summary>
        /// 写入到流的数据包绑定
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
        /// 当前的协议状态
        /// </summary>
        public ProtocolState State { get; private set; } = ProtocolState.Any;

        /// <summary>
        /// 启用压缩
        /// </summary>
        public bool Compressing { get; private set; } = false;

        /// <summary>
        /// 适配器是否在运行
        /// </summary>
        public bool Running { get; private set; }

        public int UnReadPacketCount { get; private set; }

        private event Action<Exception> Exception;

        private bool HandleSpecialPacket(Packet packet, bool sending = false)
        {
            switch (packet)
            {
                case LoginSetCompressionPacket loginSetCompressionPacket:
                    Compressing = loginSetCompressionPacket.Threshold > 0;
                    _logger.Info($"Set compression: {Compressing}");
                    return true;
                case HandshakePacket handshakePacket:
                    State = handshakePacket.NextState;
                    _logger.Debug($"Change protocol state: {State}");
                    return false;
                case LoginSuccessPacket _:
                    State = ProtocolState.Play;
                    _logger.Debug($"Change protocol state: {State}");
                    return false;
                case KeepAlivePacket keepAlivePacket:
                    if (!sending)
                        SendPacket(new KeepAliveResponsePacket { KeepAliveId = keepAlivePacket.KeepAliveId });
                    return true;
                default:
                    return false;
            }
        }

        public void Start()
        {
            if (Running) throw new ProtocolException("Adapter has already running");
            Running = true;
            _logger.Info("Adapter started");
            State = ProtocolState.Handshaking;
            new Thread(() => // receive thread
            {
                Logger.SetThreadName("ProtocolAdapterMainThread");
                lock (_runningLock)
                {
                    try
                    {
                        while (true)
                        {
                            lock (_receiveLock)
                            {
                            //try
                            //{
                            startReceive:
                                Packet packet = null;
                                try
                                {
                                    packet = Packet.ReadPacket(_receiveStream, RemoteBoundTo, () => State, () => Compressing, dp => packet = dp);
                                }
                                catch (PacketParseException)
                                {
                                    // ignore
                                }
                                //HandleSpecialPacket(packet);
                                if (HandleSpecialPacket(packet))
                                    goto startReceive;
                                lock (_receiveQueue)
                                {
                                    _receiveQueue.Enqueue(packet);
                                    UnReadPacketCount = _receiveQueue.Count;
                                }
                                //}
                                //finally
                                //{
                                //Monitor.PulseAll(_receiveQueue);
                                //}
                            }
                        }
                    }
                    catch (EndOfStreamException)
                    {
                        // ignore
                    }
                    catch (ProtocolException ex)
                    {
                        _logger.Warn(ex);
                    }
                    catch (Exception ex)
                    {
                        Exception?.Invoke(ex);
                    }
                    finally
                    {
                        _logger.Info("Protocol has been stopped");
                        Running = false;
                    }
                }
            })
            { IsBackground = true }.Start();
            new Thread(() => // send thread
            {
                Logger.SetThreadName("ProtocolAdapterSendThread");
                while (true)
                {
                    _lastSendException = null;
                    lock (_sendQueue)
                    {
                        if (!Running && _sendQueue.Count != 0)
                            break;
                        if (_sendQueue.TryDequeue(out var packet))
                        {
                            HandleSpecialPacket(packet);
                            void WriteDataPacket(DataPacket dp)
                            {
                                if (Compressing)
                                    dp.WriteCompressedToStream(_sendStream);
                                else dp.WriteToStream(_sendStream);
                                _sendStream.Flush();
                            }
                            try
                            {
                                if (packet is DataPacket dataPacket) WriteDataPacket(dataPacket);
                                else
                                {
                                    var content = new ByteArray(0);
                                    packet.WriteToStream(content);
                                    WriteDataPacket(new DataPacket(packet.PacketId, packet.BoundTo, content, State));
                                }
                            }
                            catch (Exception ex)
                            {
                                _lastSendException = ex;
                            }
                            finally
                            {
                                Monitor.Pulse(_sendQueue);
                            }
                        }
                    }
                }
                lock (_sendQueue)
                {
                    Monitor.PulseAll(_sendQueue);
                }
            })
            { IsBackground = true }.Start();
        }

        /// <summary>
        /// 等待协议适配器停止运行
        /// </summary>
        public void WaitUntilStop()
        {
            lock (_runningLock)
            {
            }
        }

        /// <summary>
        /// 进入协议适配器处理其异常直到其停止
        /// </summary>
        public void Enter()
        {
            Exception exception = null;
            void ExceptionHandler(Exception ex)
            {
                exception = ex;
            }
            Exception += ExceptionHandler;
            lock (_runningLock)
            {
            }
            Exception -= ExceptionHandler;
            if (exception != null)
                throw new AggregateException(exception);
        }

        public void Stop()
        {
            _sendStream.Dispose();
            _receiveStream.Dispose();
        }

        public Packet ReceivePacket()
        {
#if UseReceiveLock //这里不用_receiveLock，因为可能会锁死
            lock (_receiveLock)
            {
#else
            var @lock = new object();
            new Thread(() =>
            {
                lock (@lock)
                {
                    while (true)
                    {
                        lock (_receiveQueue)
                        {
                            if (UnReadPacketCount != 0 || UnReadPacketCount == 0 && !Running)
                                break;
                        }
                        Thread.Sleep(1); //cpu break
                    }
                }
            })
            { IsBackground = true }.Start();
            lock (@lock)
            {
#endif
                lock (_receiveQueue)
                {
                    _receiveQueue.TryDequeue(out var packet);
                    UnReadPacketCount = _receiveQueue.Count;
                    return packet;
                }
            }
        }

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public void SendPacket(Packet packet)
        {
            if (!Running)
                throw new ProtocolException("The adapter is not running");
            lock (_sendQueue)
            {
                _sendQueue.Enqueue(packet);
                Monitor.Wait(_sendQueue);
            }
            if (_lastSendException != null)
                throw new AggregateException(_lastSendException);
        }

#if false // old

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        /// <exception cref="ProtocolException">包类型与写入流的Bound不匹配</exception>
        public void SendPacket(Packet packet)
        {
            if (packet.BoundTo != BoundTo)
                throw new ProtocolException("packet boundTo incorrect");
            if (packet.State != State && State != ProtocolState.Any)
                throw new ProtocolException($"packet state incorrect, current state: {State}");
            var content = new ByteArray(0);
            HandleSpecialPacket(packet);
            packet.WriteToStream(content);
            content.Position = 0;
            var dataPacket = new DataPacket(packet.PacketId, packet.BoundTo, content, State);
            SendDataPacket(dataPacket);

            if (packet is KeepAliveResponsePacket)
                _logger.Debug("keep alive -> S");

            packet.OnSend(this);
            PacketSent?.Invoke(this, packet);
        }

        public void SendDataPacket(DataPacket dataPacket)
        {
            lock (_sendStream)
            {
                if (Compressing) dataPacket.WriteCompressedToStream(_sendStream);
                else dataPacket.WriteToStream(_sendStream);
                _sendStream.Flush();
            }
        }

        private void HandleSpecialPacket(Packet packet)
        {
            // _logger.Debug($"received packet: 0x{packet.PacketId.ToString("X").PadLeft(2, '0')} {packet.GetType().Name}");
            switch (packet)
            {
                case LoginSetCompressionPacket loginSetCompressionPacket:
                    Compressing = loginSetCompressionPacket.Threshold > 0;
                    _logger.Info($"Set compression: {Compressing}");
                    break;
                case HandshakePacket handshakePacket:
                    State = handshakePacket.NextState;
                    _logger.Debug($"Change protocol state: {State}");
                    break;
                case LoginSuccessPacket _:
                    State = ProtocolState.Play;
                    _logger.Debug($"Change protocol state: {State}");
                    break;
            }
        }

        /// <summary>
        /// 启动适配器
        /// </summary>
        public void Start()
        {
            if (Running) throw new ProtocolException("Adapter has already running");
            Running = true;
            _logger.Info("Adapter started");
            State = ProtocolState.Handshaking;
            new Thread(() =>
            {
                lock (_runningLock)
                {
                    Logger.SetThreadName("ProtocolAdapterThread");
                    try
                    {
                        while (true)
                        {
                            Packet packet;
                            try
                            {
                                packet = ReceivePacket();
                            }
                            catch (PacketParseException)
                            {
                                //_logger.Warn(ex.Message);
                                // ignore
                                continue;
                            }
                            catch (ProtocolException ex)
                            {
                                _logger.Warn(ex.Message);
                                continue;
                            }
                            catch (EndOfStreamException)
                            {
                                Stopped?.Invoke(this, EventArgs.Empty);
                                _logger.Info("Adapter has been stopped. EOF");
#if DEBUG
                                throw;
#else
                                return;
#endif
                            }
                            catch
                            {
                                Stopped?.Invoke(this, EventArgs.Empty);
                                if (!_stopping)
                                {
                                    throw;
                                }
                                _logger.Info("Adapter has been stopped.");
                                return;
                            }

                            HandleSpecialPacket(packet);
                            PacketReceived?.Invoke(this, packet);
                        }
                    }
                    catch
                    {
                        _logger.Warn("Adapter stoped with exception.");
                        throw;
                    }
                    finally
                    {
                        Running = false;
                        Stopped?.Invoke(this, EventArgs.Empty);
                    }
                }
            }).Start();
            //Thread.Sleep(1);
            Started?.Invoke(this, EventArgs.Empty);
        }

        public void WaitUntilStop()
        {
            lock (_runningLock)
            {
            }
        }

        private bool _stopping = false;

        /// <summary>
        /// 停止适配器并关闭流
        /// </summary>
        public void Stop()
        {
            if (_stopping)
            {
                lock (_baseStream)
                {
                }
                return;
            }
            _stopping = true;
            lock (_baseStream)
            {
                _baseStream.Close();
            }
            _stopping = false;
        }

        private Packet ReceivePacket()
        {
            Packet packet = null;
            bool exited = false;
            try
            {
                lock (_receiveStream)
                {
                    packet = Packet.ReadPacket(_receiveStream, RemoteBoundTo, () => State, () => Compressing, dataPacket =>
                    {
                        UnregisteredPacketReceived?.Invoke(this, (dataPacket, p =>
                        {
                            if (exited)
                                throw new InvalidOperationException("This packet request has already handled.");
                            packet = p;
                        }
                        ));
                    });
                    Monitor.Wait(_receiveStream);
                }
            }
            catch (PacketParseException)
            {
                if (packet == null)
                    throw;
            }

            exited = true;
            return packet;
        }

        public Packet ReceiveSinglePacket()
        {
            lock (_receiveStream)
            {
                if (_lastPacket == null)
                    throw new ProtocolException("Protocol stopped.");
                return _lastPacket;
            }
        }

        public T ReceiveSinglePacket<T>() where T : Packet
        {
            while (true)
            {
                var packet = ReceiveSinglePacket();
                if (packet is T t)
                {
                    return t;
                }
            }
        }

        public void HandleReceiveSinglePacket(Action<Packet> handler)
        {
            var packet = ReceiveSinglePacket();
            handler(packet);
        }

        public void HandleReceiveSinglePacket<T>(Action<T> handler) where T : Packet
        {
            var packet = ReceiveSinglePacket<T>();
            handler(packet);
        }

        /// <summary>
        /// 接收到数据包
        /// </summary>
        public event EventHandler<Packet> PacketReceived;

        /// <summary>
        /// 发送了数据包
        /// </summary>
        public event EventHandler<Packet> PacketSent;

        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler<(DataPacket dataPacket, Action<Packet> handlePacket)> UnregisteredPacketReceived;

        /// <summary>
        /// 处理指定数据包
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
        /// 取消处理指定数据包
        /// </summary>
        /// <param name="result">处理数据包返回的结果</param>
        public void UnHandlePacket(PacketHandleResult result)
        {
            PacketReceived -= result.Handler;
        }

        /// <summary>
        /// 处理发送了某个数据包
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
        /// 取消处理发送了某个数据包
        /// </summary>
        /// <param name="result">处理发送了某个数据包返回的结果</param>
        public void UnHandleSendPacket(PacketHandleResult result)
        {
            PacketSent -= result.Handler;
        }

        /// <summary>
        /// 数据包处理返回的结果
        /// </summary>
        public class PacketHandleResult
        {
            internal readonly EventHandler<Packet> Handler;

            internal PacketHandleResult(EventHandler<Packet> handler)
            {
                Handler = handler;
            }
        }
#endif
    }
}