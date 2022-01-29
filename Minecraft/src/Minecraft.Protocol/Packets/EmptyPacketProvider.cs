using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Minecraft.Protocol.Packets
{
    public class EmptyPacketProvider : IPacketProvider
    {
        private class PacketInfo
        {
            public int PacketId;
            public PacketBoundTo BoundTo;
            public ProtocolState State;
            public Func<IPacket> Constructor;
#if DEBUG
            public string PacketName;
#endif

            public override string ToString()
            {
                return $"0x{PacketId:X2}, {BoundTo}, {State}: {PacketName}";
            }
        }

        private readonly List<PacketInfo> _packetInfo = new List<PacketInfo>();

        public EmptyPacketProvider AddPacket<T>() where T : IPacket, new()
        {
            var packet = new T();
            _packetInfo.Add(new PacketInfo
            {
                PacketId = packet.PacketId,
                BoundTo = packet.BoundTo,
                State = packet.State,
#if DEBUG
                PacketName = typeof(T).Name,
#endif
                Constructor = () => new T()
            });
            return this;
        }

        public EmptyPacketProvider AutoSearchPacketTypes(string typePrefix = null, Assembly assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes().Where(type =>
                   type.FullName.StartsWith(typePrefix ?? "")
                && type.FullName.EndsWith("Packet")).ToArray())
            {
                var constructor = type.GetConstructor(new Type[0]);
                var packet = (IPacket)constructor.Invoke(null);
                var info = new PacketInfo
                {
                    PacketId = packet.PacketId,
                    BoundTo = packet.BoundTo,
                    State = packet.State,
#if DEBUG
                    PacketName = type.Name,
#endif
                    Constructor = () => (IPacket)constructor.Invoke(null)
                };
                _packetInfo.Add(info);
            }
            return this;
        }

        public bool TryCreatePacket(int packetId, PacketBoundTo boundTo, ProtocolState state, out IPacket packet)
        {
            packet = _packetInfo.FirstOrDefault(info =>
                info.PacketId == packetId &&
                info.BoundTo == boundTo &&
                (info.State == state || info.State == ProtocolState.Any)
            )?.Constructor();
            return packet != null;
        }
    }
}
