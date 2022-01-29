//#define LogSendReceivePacket

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Minecraft.Extensions;
using Minecraft.Protocol.MCVersions.MC1171.Packets.Client;
using Minecraft.Protocol.MCVersions.MC1171.Packets.Server;
using Minecraft.Protocol.Packets;

namespace Minecraft.Protocol.MCVersions.MC1171
{
    public class MCVer756ProtocolAdapter : ProtocolAdapterBase
    {
        private static readonly Logger<MCVer756ProtocolAdapter> _logger = Logger.GetLogger<MCVer756ProtocolAdapter>();
        private static readonly IPacketProvider provider = new EmptyPacketProvider().AutoSearchPacketTypes("Minecraft.Protocol.MCVersions.MC1171.Packets");

        public MCVer756ProtocolAdapter(Stream baseStream, PacketBoundTo boundTo) : base(baseStream, boundTo)
        {
        }

        public override IPacketCodec DefaultCodec { get; } = new DefaultPacketCodec();

        public override IPacketProvider PacketProvider => provider;

        protected override bool OnPacketReceived(IPacket packet)
        {
            //if(!(packet is DataPacket))
            _logger.Info(packet.ToString());
            switch (packet)
            {
                case LoginSetCompressionPacket loginSetCompressionPacket:
                    CompressThreshold = loginSetCompressionPacket.Threshold;
                    _logger.Info($"Set compression: {Compressed}, Threshold: {CompressThreshold}");
                    return false;
                case LoginSuccessPacket _:
                    State = ProtocolState.Play;
                    _logger.Debug($"Change protocol state: {State}");
                    return true;
                case KeepAlivePacket keepAlivePacket:
                    SendImportantPacket(new KeepAliveResponsePacket { KeepAliveId = keepAlivePacket.KeepAliveId });
                    return false;
                case DisconnectPacket _:
                case LoginDisconnectPacket _:
                    Stop();
                    return true;
            }
            return base.OnPacketReceived(packet);
        }

        protected override bool OnSendingPacket(IPacket packet)
        {
            switch (packet)
            {
                case HandshakePacket handshakePacket:
                    State = handshakePacket.NextState;
                    _logger.Debug($"Change protocol state: {State}");
                    break;
            }
            _logger.Info(packet.GetPropertyInfoString());
            return base.OnSendingPacket(packet);
        }

        protected override IPacket OnUnknownPacketReceived(DataPacket packet)
        {
            return base.OnUnknownPacketReceived(packet);
        }
    }
}