using Minecraft.Protocol.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Minecraft.Protocol
{
    public abstract class ProtocolAdapterBase : IDisposable
    {
        private static readonly Logger<ProtocolAdapterBase> _logger = Logger.GetLogger<ProtocolAdapterBase>();

        public ProtocolAdapterBase(Stream baseStream, PacketBoundTo boundTo)
        {
            BaseStream = baseStream;
            BufferedReadStream = new BufferedStream(baseStream);
            BufferedWriteStream = new BufferedStream(baseStream);
            _rawReadCodec = DefaultCodec.Clone(BufferedReadStream);
            _rawWriteCodec = DefaultCodec.Clone(BufferedWriteStream);
            BoundTo = boundTo;
            RemoteBoundTo = boundTo switch
            {
                PacketBoundTo.Client => PacketBoundTo.Server,
                PacketBoundTo.Server => PacketBoundTo.Client,
                _ => default
            };
        }

        private bool _running;
        private int _waitReceiveCount;
        private int _waitSendCount;
        private readonly Queue<IPacket> _receivePacketQueue = new Queue<IPacket>(),
                                        _sendPacketQueue = new Queue<IPacket>(),
                                        _importantPacketQueue = new Queue<IPacket>();
        private readonly IPacketCodec _rawReadCodec, _rawWriteCodec;

        public PacketBoundTo BoundTo { get; private set; }
        public PacketBoundTo RemoteBoundTo { get; private set; }
        public abstract IPacketCodec DefaultCodec { get; }
        public abstract IPacketProvider PacketProvider { get; }
        public Stream BaseStream { get; }
        public BufferedStream BufferedReadStream { get; }
        public BufferedStream BufferedWriteStream { get; }
        public int CompressThreshold { get; set; }
        public bool Compressed => CompressThreshold != 0;
        public bool Running => _running;

        public ProtocolState State { get; set; } = ProtocolState.Handshaking;

        /// <summary>
        /// 接收到数据包
        /// </summary>
        /// <param name="packet"></param>
        /// <returns>是否保留数据包，如果为 true 则保留，否则丢弃</returns>
        protected virtual bool OnPacketReceived(IPacket packet)
        {
            return true;
        }

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="packet"></param>
        /// <returns>是否继续发送数据包，如果为 true 则保留，否则丢弃</returns>
        protected virtual bool OnSendingPacket(IPacket packet)
        {
            return true;
        }

        /// <summary>
        /// 发送了数据包
        /// </summary>
        /// <param name="packet"></param>
        protected virtual void OnPacketSent(IPacket packet)
        {
        }

        protected virtual IPacket OnUnknownPacketReceived(DataPacket packet)
        {
            return packet;
        }

        public event EventHandler<Exception> ExceptionOccurred;

        public IPacket ReceivePacket()
        {
            if (!_running) Start();
            lock (_receivePacketQueue)
            {
                if (_receivePacketQueue.Count == 0)
                {
                    _waitReceiveCount++;
                    Monitor.Wait(_receivePacketQueue);
                    _waitReceiveCount--;
                }
                _receivePacketQueue.TryDequeue(out var packet);
                return packet;
            }
        }

        public void SendPacket(IPacket packet)
        {
            if (packet is null)
                throw new ArgumentNullException(nameof(packet));
            if (!_running) Start();

            if (OnSendingPacket(packet))
                _sendPacketQueue.Enqueue(packet);
        }

        public void SendImportantPacket(IPacket packet)
        {
            if (packet is null)
                throw new ArgumentNullException(nameof(packet));
            if (!_running) Start();

            if (OnSendingPacket(packet))
                _importantPacketQueue.Enqueue(packet);
        }

        public void Start(bool receiveThread = true, bool sendThread = true)
        {
            if (!receiveThread && !sendThread) return;
            if (_running) return;
            _running = true;
            if (receiveThread)
                ThreadHelper.StartThread(PacketReceivingThread, "NetworkThread-R", true);
            if (sendThread)
                ThreadHelper.StartThread(PacketSendingThread, "NetworkThread-W", true);
        }

        private void PacketSendingThread()
        {
            try
            {
                while (_running)
                {
                    var send = false;
                    while (_sendPacketQueue.Count != 0 || _importantPacketQueue.Count != 0)
                    {
                        IPacket packet = null;
                        var flag = false;

                        lock (_importantPacketQueue)
                        {
                            if (_importantPacketQueue.Count != 0)
                            {
                                packet = _importantPacketQueue.Peek();
                                flag = false;
#if LogSendReceivePacket
                                _logger.Error($"Sent important packet 0x{Convert.ToString(packet.PacketId, 16).PadLeft(2, '0')}, {packet.GetType().FullName}");
#endif
                            }
                        }

                        if (packet == null)
                        {
                            lock (_sendPacketQueue)
                            {
                                //Debug.Assert(_sendPacketQueue.Peek() != null);
                                //something wrong here

                                packet = _sendPacketQueue.Peek();
                                flag = true;
#if LogSendReceivePacket
                                _logger.Warn($"Sent packet 0x{Convert.ToString(packet.PacketId, 16).PadLeft(2, '0')}, {packet.GetType().FullName}");
#endif
                                if (packet == null)
                                    continue;
                            }
                        }

                        var dataPacket = new DataPacket(DefaultCodec);
                        dataPacket.ReadFromPacket(packet);
                        dataPacket.ResetPosition();
                        dataPacket.WriteToStream(_rawWriteCodec, () => CompressThreshold);

                        send = true;

                        if (!flag) _importantPacketQueue.Dequeue();
                        else _sendPacketQueue.Dequeue();
                    }
                    if (send)
                    {
                        BufferedWriteStream.Flush();
                        lock (_sendPacketQueue)
                        {
                            if (_waitSendCount != 0)
                            {
                                Monitor.PulseAll(_sendPacketQueue);
                            }
                            _waitSendCount = 0;
                        }
                    }
                    Thread.Sleep(1); // cpu break
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                ExceptionOccurred?.Invoke(this, ex);
            }
            finally
            {
                _running = false;
                State = ProtocolState.Closed;
            }
        }

        private void PacketReceivingThread()
        {
            try
            {
                while (_running)
                {
                    var datapacket = new DataPacket(DefaultCodec);

                    datapacket.ReadFromStream(_rawReadCodec, RemoteBoundTo, () => CompressThreshold, () => State);

                    if (!PacketProvider.TryCreatePacket(datapacket.PacketId, datapacket.BoundTo, datapacket.State, out var packet)
                        && ((packet = OnUnknownPacketReceived(datapacket)) is DataPacket
                            || packet is null))
                    {
                        _logger.Warn($"Unknown packet 0x{datapacket.PacketId:X2}");
                        continue;
                    }

                    packet.ReadFromStream(datapacket.Content);

                    if (!OnPacketReceived(packet))
                        continue;

                    lock (_receivePacketQueue)
                    {
                        _receivePacketQueue.Enqueue(packet);

                        if (_waitReceiveCount != 0)
                        {
                            Monitor.Pulse(_receivePacketQueue);
                        }
                    }
                }
            }
            catch (EndOfStreamException)
            {
                _receivePacketQueue.Enqueue(null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                ExceptionOccurred?.Invoke(this, ex);
            }
            finally
            {
                lock (_receivePacketQueue)
                {
                    if (_waitReceiveCount != 0)
                    {
                        Monitor.PulseAll(_receivePacketQueue);
                    }
                }
                _running = false;
                State = ProtocolState.Closed;
            }
        }

        public void Stop()
        {
            if (!_running) return;
            _running = false;
        }

        public void WaitUntilAllPacketsSent()
        {
            lock (_sendPacketQueue)
            {
                if (_sendPacketQueue.Count != 0)
                {
                    _waitSendCount++;
                    Monitor.Wait(_sendPacketQueue);
                }
            }
        }

        public void Close()
        {
            WaitUntilAllPacketsSent(); //sync
            BaseStream.Close();
            State = ProtocolState.Closed;
        }

        public void Dispose()
        {
            BufferedReadStream.Dispose();
            BaseStream.Close();
            State = ProtocolState.Closed;
        }
    }
}
