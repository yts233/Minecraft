//#define LogSendReceivePacket

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
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
    /// <remarks>Minecraft protocol version: 756</remarks>
    public class ProtocolAdapter : IDisposable
    {
        private static readonly Logger<ProtocolAdapter> _logger = Logger.GetLogger<ProtocolAdapter>();

        private readonly BufferedStream _receiveStream;
        private readonly BufferedStream _sendStream;

        /// <summary>
        /// 创建协议适配器
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="boundTo">写入流绑定至</param>
        public ProtocolAdapter(Stream stream, PacketBoundTo boundTo)
        {
            //_baseStream = stream;
            _receiveStream = new BufferedStream(stream);
            _sendStream = new BufferedStream(stream);
            BoundTo = boundTo;
            RemoteBoundTo = boundTo switch
            {
                PacketBoundTo.Client => PacketBoundTo.Server,
                PacketBoundTo.Server => PacketBoundTo.Client,
                _ => default
            };
        }

        /// <summary>
        /// 写入到流的数据包绑定
        /// </summary>
        public PacketBoundTo BoundTo { get; }

        /// <summary>
        /// 从流读出的数据包绑定
        /// </summary>
        public PacketBoundTo RemoteBoundTo { get; }

        /// <summary>
        /// 当前的协议状态
        /// </summary>
        public ProtocolState State { get; set; } = ProtocolState.Any;

        /// <summary>
        /// 启用压缩
        /// </summary>
        public bool Compressing { get; set; } = false;

        /// <summary>
        /// 压缩门槛
        /// </summary>
        /// <remarks>当数据包DataLength小于此门槛是不使用压缩</remarks>
        public int Threshold { get; set; } = 0;

        public bool AutoHandleSpecialPacket { get; set; } = true;

        public bool AutoSendSpecialPacket { get; set; } = true;

        private void HandleSpecialPacket(Packet packet, bool sending = false)
        {
            switch (packet)
            {
                case LoginSetCompressionPacket loginSetCompressionPacket:
                    Compressing = loginSetCompressionPacket.Threshold > 0;
                    Threshold = loginSetCompressionPacket.Threshold;
                    _logger.Info($"Set compression: {Compressing}, Threshold: {Threshold}");
                    break;
                case HandshakePacket handshakePacket:
                    State = handshakePacket.NextState;
                    _logger.Debug($"Change protocol state: {State}");
                    break;
                case LoginSuccessPacket _:
                    State = ProtocolState.Play;
                    _logger.Debug($"Change protocol state: {State}");
                    break;
                case KeepAlivePacket keepAlivePacket:
                    if (!sending && AutoSendSpecialPacket)
                    {
                        WriteImportantPacket(new KeepAliveResponsePacket { KeepAliveId = keepAlivePacket.KeepAliveId });
                    }
                    break;
            }
        }

        private bool _running;
        private int _waitReceiveCount;
        private int _waitSendCount;
        private readonly Queue<Packet> _receivePacketQueue = new Queue<Packet>(),
                                       _sendPacketQueue = new Queue<Packet>(),
                                       _importantPacketQueue = new Queue<Packet>();

        public void Start()
        {
            if (_running)
            {
                return;
            }
            _running = true;
            ThreadHelper.StartThread(PacketReceivingThread, "NetworkThread-R", true);
            ThreadHelper.StartThread(PacketSendingThread, "NetworkThread-W", true);
        }
        public void Stop()
        {
            if (!_running)
            {
                return;
            }
            _running = false;
        }

        public event EventHandler<Exception> ExceptionOccurred;
        private void PacketReceivingThread()
        {
            try
            {
                while (_running)
                {
                    Packet.TryReadParsedPacket(_receiveStream, RemoteBoundTo, () => State == ProtocolState.Any ? ProtocolState.Handshaking : State, () => Compressing, () => Threshold, out var packet);
                    if (AutoHandleSpecialPacket)
                    {
                        HandleSpecialPacket(packet);
                    }

                    lock (_receivePacketQueue)
                    {
                        _receivePacketQueue.Enqueue(packet);
#if LogSendReceivePacket
                        if (packet is KeepAlivePacket)
                        {
                            _logger.Warn("Get keep alive packet.");
                        }
                        if (!(packet is DataPacket || packet is ChunkDataPacket))
                            _logger.Debug($"Received packet 0x{Convert.ToString(packet.PacketId, 16).PadLeft(2, '0')}, {packet.GetType().FullName}");
#endif
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
        private void PacketSendingThread()
        {
            try
            {
                while (_running)
                {
                    var send = false;
                    while (_sendPacketQueue.Count != 0 || _importantPacketQueue.Count != 0)
                    {
                        Packet packet = null;

                        lock (_importantPacketQueue)
                        {
                            if (_importantPacketQueue.Count != 0)
                            {
                                packet = _importantPacketQueue.Dequeue();
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

                                packet = _sendPacketQueue.Dequeue();
#if LogSendReceivePacket
                                _logger.Warn($"Sent packet 0x{Convert.ToString(packet.PacketId, 16).PadLeft(2, '0')}, {packet.GetType().FullName}");
#endif
                                if (packet == null)
                                    continue;
                            }
                        }

                        void WriteDataPacket(DataPacket dp)
                        {
                            if (Compressing)
                            {
                                dp.WriteCompressedToStream(_sendStream, Threshold);
                            }
                            else dp.WriteToStream(_sendStream);
                            send = true;
                        }

                        if (packet is DataPacket dataPacket)
                        {
                            WriteDataPacket(dataPacket);
                        }
                        else
                        {
                            var content = new ByteArray(0);
                            packet.WriteToStream(content);
                            WriteDataPacket(new DataPacket(packet.PacketId, packet.BoundTo, content, State));
                        }

                        if (AutoHandleSpecialPacket)
                        {
                            HandleSpecialPacket(packet, true);
                        }
                    }
                    if (send)
                    {
                        lock (_sendPacketQueue)
                        {
                            if (_waitSendCount != 0)
                            {
                                Monitor.PulseAll(_sendPacketQueue);
                            }
                            _waitSendCount = 0;
                        }
                        _sendStream.Flush();
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

        public void WritePacket(Packet packet)
        {

            if (!_running)
            {
                Start();
            }
            if (State == ProtocolState.Any)
            {
                State = packet.State;
            }

            if (packet is null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (AutoHandleSpecialPacket)
            {
                HandleSpecialPacket(packet, true);
            }

            _sendPacketQueue.Enqueue(packet);
        }

        public void WriteImportantPacket(Packet packet)
        {

            if (!_running)
            {
                Start();
            }
            if (State == ProtocolState.Any)
            {
                State = packet.State;
            }

            if (packet is null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (AutoHandleSpecialPacket)
            {
                HandleSpecialPacket(packet, true);
            }

            _importantPacketQueue.Enqueue(packet);
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

        public Packet ReadPacket()
        {
            if (!_running)
            {
                Start();
            }
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

        public void Close()
        {
            WaitUntilAllPacketsSent(); //sync

            _sendStream.Close();
            _receiveStream.Close();
            State = ProtocolState.Closed;
        }

        public void Dispose()
        {
            _sendStream.Dispose();
            _receiveStream.Dispose();
            State = ProtocolState.Closed;
        }
    }
}