using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Minecraft.Protocol.Data;
using Minecraft.Protocol.Packets.Client;
using Minecraft.Protocol.Packets.Server;

namespace Minecraft.Protocol.Packets
{
    /// <summary>
    ///     数据包
    /// </summary>
    public abstract class Packet
    {
        //注册的数据包
        private static readonly ICollection<RegisteredPacket> RegisteredPackets = new List<RegisteredPacket>();

        static Packet()
        {
            //Server side
            Register<StateResponsePacket>();
            Register<StatePongPacket>();
            Register<EntityMovementPacket>();

            //Client side
            Register<HandshakePacket>();
            Register<StateRequestPacket>();
            Register<StatePingPacket>();
        }

        /// <summary>
        ///     包ID
        /// </summary>
        public abstract int PacketId { get; }

        /// <summary>
        ///     包源
        /// </summary>
        public abstract PacketOrigin Origin { get; }

        /// <summary>
        ///     协议状态
        /// </summary>
        public abstract ProtocolState State { get; }

        /// <summary>
        ///     注册数据包
        /// </summary>
        /// <param name="constructor">Packet构造器</param>
        public static void Register(Func<Packet> constructor) //使用委托提升性能
        {
            var packet = constructor();
            //Logger.Debug<Packet>("register packet " + packet.GetType().FullName);
            RegisteredPackets.Add(new RegisteredPacket(packet.PacketId, packet.Origin, packet.State, constructor));
        }

        /// <summary>
        ///     注册数据包
        /// </summary>
        /// <typeparam name="T">数据包的类型</typeparam>
        public static void Register<T>() where T : Packet, new()
        {
            Register(() => new T());
        }

        /// <summary>
        ///     创建数据包
        /// </summary>
        /// <param name="packetId">包Id</param>
        /// <param name="origin">包源</param>
        /// <param name="state">协议状态</param>
        /// <returns></returns>
        /// <exception cref="PacketParseException">指定数据包没有注册</exception>
        public static Packet CreatePacket(int packetId, PacketOrigin origin, ProtocolState state)
        {
            //Logger.Debug<Packet>(
            //    $"CreatePacket: packetId: 0x{packetId.ToString("X").PadLeft(2, '0')}, origin: {origin}, state: {state}");
            Packet result;
            try
            {
                result = (from packet in RegisteredPackets
                    where packet.PacketId == packetId
                    where packet.Origin == origin
                    where packet.State == state
                    select packet).First().CreateInstance();
            }
            catch (InvalidOperationException)
            {
                throw new PacketParseException(
                    $"packet 0x{packetId.ToString("X").PadLeft(2, '0')} hadn't been registered.");
            }

            return result;
        }

        /// <summary>
        ///     读取数据包
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="origin"></param>
        /// <param name="state"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        private static DataPacket ReadDataPacket(Stream stream, PacketOrigin origin,
            ProtocolState state = ProtocolState.Any,
            bool compressed = false)
        {
            var content = PacketHelper.GetContent(null, stream);
            var length = content.Read<VarInt>();
            return (DataPacket) new DataPacket(origin, state)
                .ReadFromStream(new ByteArray(
                    compressed
                        ? new DeflateStream(new ByteArray(content, length), CompressionMode.Decompress)
                        : (Stream) content, length));
        }

        public static Packet ReadPacket(Stream stream, PacketOrigin origin, ProtocolState state = ProtocolState.Any,
            bool compressed = false)
        {
            return ReadDataPacket(stream, origin, state, compressed).Parse();
        }

        /// <summary>
        ///     用指定类型读取数据包
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="origin"></param>
        /// <param name="compressed"></param>
        /// <param name="state"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReadPacket<T>(Stream stream, PacketOrigin origin, ProtocolState state = ProtocolState.Any,
            bool compressed = false) where T : Packet, new()
        {
            return ReadDataPacket(stream, origin, state, compressed).Parse<T>();
        }

        /// <summary>
        ///     从流读入
        /// </summary>
        /// <param name="content">流</param>
        protected abstract void _ReadFromStream(ByteArray content);

        /// <summary>
        ///     从流读入
        /// </summary>
        /// <returns>The from stream.</returns>
        /// <param name="stream">Stream.</param>
        public Packet ReadFromStream(Stream stream)
        {
            _ReadFromStream(this.GetContent(stream));
            return this;
        }

        /// <summary>
        ///     写入到流
        /// </summary>
        /// <param name="content">流</param>
        protected abstract void _WriteToStream(ByteArray content);

        /// <summary>
        ///     写入到流
        /// </summary>
        /// <returns>The to stream.</returns>
        /// <param name="stream">Stream.</param>
        public Packet WriteToStream(Stream stream)
        {
            _WriteToStream(this.GetContent(stream));
            return this;
        }

        /// <summary>
        ///     从流读入
        /// </summary>
        /// <param name="stream"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ReadFromStream<T>(Stream stream) where T : Packet
        {
            return (T) ReadFromStream(stream);
        }

        private class RegisteredPacket
        {
            private readonly Func<Packet> _constructor;

            public RegisteredPacket(int packetId, PacketOrigin origin, ProtocolState state, Func<Packet> constructor)
            {
                PacketId = packetId;
                Origin = origin;
                State = state;
                _constructor = constructor;
            }

            public int PacketId { get; }
            public PacketOrigin Origin { get; }
            public ProtocolState State { get; }

            public Packet CreateInstance()
            {
                return _constructor();
            }
        }
    }
}