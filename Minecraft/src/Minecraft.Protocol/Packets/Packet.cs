//#define LogRegisterPacketDebug
//#define LogNotRegisteredPacketDebug
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Minecraft.Protocol.Data;

namespace Minecraft.Protocol.Packets
{
    /// <summary>
    /// 数据包
    /// </summary>
    public abstract class Packet
    {
        private static readonly Logger<Packet> _logger = Logger.GetLogger<Packet>();
        //注册的数据包
        private static readonly ICollection<RegisteredPacket> RegisteredPackets = new List<RegisteredPacket>();

        static Packet()
        {
            var assembly = typeof(Packet).Assembly;
            foreach (var packetConstructor in assembly.GetTypes()
                .Where(type =>
                    type.IsClass
                    && !type.IsAbstract
                    && (type.FullName.StartsWith("Minecraft.Protocol.Packets.Client")
                        || type.FullName.StartsWith("Minecraft.Protocol.Packets.Server"))
                    && type.FullName.EndsWith("Packet"))
                .Select(type => type.GetConstructors()
                    .FirstOrDefault(constructor => constructor.GetParameters().Length == 0))
                .Where(type => type != null))
            {
                var constructor = packetConstructor;
                Register(() =>
                {
                    return (Packet)constructor.Invoke(new object[0]);
                });
            }
        }
        /// <summary>
        /// 包ID
        /// </summary>
        public abstract int PacketId { get; }

        /// <summary>
        /// 包绑定至
        /// </summary>
        public abstract PacketBoundTo BoundTo { get; }

        /// <summary>
        /// 协议状态
        /// </summary>
        public abstract ProtocolState State { get; }

        /// <summary>
        /// 注册数据包
        /// </summary>
        /// <param name="constructor">Packet构造器</param>
        public static void Register(Func<Packet> constructor) //使用委托提升性能
        {
            var packet = constructor();
#if LogRegisterPacketDebug
            _logger.Debug("register packet " + packet.GetType().FullName);
#endif
            RegisteredPackets.Add(new RegisteredPacket(packet.PacketId, packet.BoundTo, packet.State, constructor));
        }

        /// <summary>
        /// 注册数据包
        /// </summary>
        /// <typeparam name="T">数据包的类型</typeparam>
        public static void Register<T>() where T : Packet, new()
        {
            Register(() => new T());
        }

        /// <summary>
        /// 创建数据包
        /// </summary>
        /// <param name="packetId">包Id</param>
        /// <param name="boundTo">绑定至</param>
        /// <param name="state">协议状态</param>
        /// <returns></returns>
        /// <exception cref="PacketParseException">指定数据包没有注册</exception>
        public static Packet CreatePacket(int packetId, PacketBoundTo boundTo, ProtocolState state)
        {
            //_ = Logger.Debug<Packet>($"CreatePacket: packetId: 0x{packetId.ToString("X").PadLeft(2, '0')}, boundTo: {boundTo}, state: {state}");
            Packet result;
            result = RegisteredPackets.FirstOrDefault(packet => packet.PacketId == packetId
                                                       && packet.BoundTo == boundTo
                                                       && packet.State == state)?.CreateInstance();
            if (result == null)
            {
#if LogNotRegisteredPacketDebug
                    _logger.Debug($"packet 0x{packetId.ToString("X2")}, bound to {boundTo} hadn't been registered.");
#endif
                throw new PacketParseException(
                        $"packet 0x{packetId.ToString("X2")}, bound to {boundTo} hadn't been registered.");
            }
            //_ = Logger.Debug<Packet>(result.GetType().FullName);

            return result;
        }

        public event EventHandler<Packet> SentByAdapter;

        internal void OnSend(object sender)
        {
            SentByAdapter?.Invoke(sender, this);
        }

        /// <summary>
        /// 读取数据包
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="boundTo"></param>
        /// <param name="state"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public static DataPacket ReadDataPacket(Stream stream, PacketBoundTo boundTo,
            Func<ProtocolState> state, Func<bool> compressed, Func<int> threshold)
        {
            var content = PacketHelper.GetContent(null, stream);
            var packetLength = content.ReadVarInt();
            try
            {
                var dataStream = new ByteArray(content, packetLength);
                var state_ = state();
                var compressed_ = compressed();
                var threshold_ = threshold();
                if (compressed_)
                {
                    var dataLength = dataStream.ReadVarInt();
                    if (dataLength != 0)
                    {
                        if (dataLength < threshold_)
                            throw new ProtocolException($"invalid packet: dataLength: {dataLength} should be greater than threshold: {threshold}");
                        using var compressedStream = new InflaterInputStream(dataStream);
                        using var buffer = new ByteArray(compressedStream, dataLength);
                        return (DataPacket)new DataPacket(boundTo, state_)
                              .ReadFromStream(buffer);
                    }
                }
                return (DataPacket)new DataPacket(boundTo, state_)
                    .ReadFromStream(dataStream);
            }
            catch (Exception ex)
            {
                throw new ProtocolException("Error while read a packet", ex);
            }
        }

        public static Packet ReadPacket(Stream stream, PacketBoundTo origin, Func<ProtocolState> state,
           Func<bool> compressed, Func<int> threshold, Func<DataPacket, Packet> unregisteredPacketReceivedHandler = null)
        {
            if (unregisteredPacketReceivedHandler == null)
                return ReadDataPacket(stream, origin, state, compressed, threshold).Parse();
            else
            {
                var dataPacket = ReadDataPacket(stream, origin, state, compressed, threshold);
                try
                {
                    return dataPacket.Parse();
                }
                catch (PacketParseException)
                {
                    return unregisteredPacketReceivedHandler.Invoke(dataPacket);
                }
            }
        }

        /// <summary>
        /// 用指定类型读取数据包
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="origin"></param>
        /// <param name="compressed"></param>
        /// <param name="state"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReadPacket<T>(Stream stream, PacketBoundTo origin, Func<ProtocolState> state,
           Func<bool> compressed, Func<int> threshold) where T : Packet, new()
        {
            return ReadDataPacket(stream, origin, state, compressed, threshold).Parse<T>();
        }

        /// <summary>
        /// 从流读入
        /// </summary>
        /// <param name="content">流</param>
        protected abstract void ReadFromStream_(ByteArray content);

        /// <summary>
        /// 从流读入
        /// </summary>
        /// <returns>The from stream.</returns>
        /// <param name="stream">Stream.</param>
        public Packet ReadFromStream(Stream stream)
        {
            ReadFromStream_(this.GetContent(stream));
            return this;
        }

        /// <summary>
        /// 将此数据包写入到流
        /// </summary>
        /// <param name="content">流</param>
        protected abstract void WriteToStream_(ByteArray content);

        /// <summary>
        /// 将此数据包写入到流
        /// </summary>
        /// <returns>The to stream.</returns>
        /// <param name="stream">Stream.</param>
        public Packet WriteToStream(Stream stream)
        {
            VerifyValues();
            WriteToStream_(this.GetContent(stream));
            return this;
        }

        private class RegisteredPacket
        {
            private readonly Func<Packet> _constructor;

            public RegisteredPacket(int packetId, PacketBoundTo boundTo, ProtocolState state, Func<Packet> constructor)
            {
                PacketId = packetId;
                BoundTo = boundTo;
                State = state;
                _constructor = constructor;
            }

            public int PacketId { get; }
            public PacketBoundTo BoundTo { get; }
            public ProtocolState State { get; }

            public Packet CreateInstance()
            {
                return _constructor();
            }
        }

        protected virtual void VerifyValues()
        {
        }
    }
}