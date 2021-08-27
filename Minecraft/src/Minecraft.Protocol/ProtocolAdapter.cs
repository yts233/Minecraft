//#define LogPacket
using System;
using System.Collections.Generic;
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
        //private readonly Stream _baseStream;
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
                        WritePacket(new KeepAliveResponsePacket { KeepAliveId = keepAlivePacket.KeepAliveId });
                    break;
            }
        }

        public Packet ReadPacket()
        {
            Packet packet = null;
            try
            {
                lock (_receiveStream)
                {
                    packet = Packet.ReadPacket(_receiveStream, RemoteBoundTo, () => State == ProtocolState.Any ? ProtocolState.Handshaking : State, () => Compressing, () => Threshold, dp => dp);
#if LogPacket
                    var s = packet.GetPropertyInfoString();
                    if (s.Length > 512)
                        s = packet.GetType().FullName;
                    _logger.Info($"S->C {s}");
                    //_logger.Info($"S->C {packet.GetType().FullName}");
#endif
                    if (AutoHandleSpecialPacket)
                        HandleSpecialPacket(packet);
                    return packet;
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
                return null;
            }
            //catch (EndOfStreamException)
            //{
            //    return null;
            //}
            finally
            {
                if (packet == null)
                    State = ProtocolState.Closed;
            }
        }

        public async Task<Packet> ReadPacketAsync()
        {
            await Task.Yield();
            return ReadPacket();
        }

        public void WritePacket(Packet packet)
        {
            lock (_sendStream)
            {
                void WriteDataPacket(DataPacket dp)
                {
                    if (Compressing)
                        dp.WriteCompressedToStream(_sendStream, Threshold);
                    else dp.WriteToStream(_sendStream);
                    _sendStream.Flush();
                }

                if (packet is DataPacket dataPacket) WriteDataPacket(dataPacket);
                else
                {
#if LogPacket
                    var s = packet.GetPropertyInfoString();
                    if (s.Length > 512)
                        s = packet.GetType().FullName;
                    _logger.Info($"C->S {s}");
                    //_logger.Info($"C->S {packet.GetType().FullName}");
#endif
                    var content = new ByteArray(0);
                    packet.WriteToStream(content);
                    WriteDataPacket(new DataPacket(packet.PacketId, packet.BoundTo, content, State));
                }

                if (AutoHandleSpecialPacket)
                    HandleSpecialPacket(packet, true);
            }
        }

        public async Task WritePacketAsync(Packet packet)
        {
            await Task.Yield();
            WritePacket(packet);
        }

        public void Close()
        {
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