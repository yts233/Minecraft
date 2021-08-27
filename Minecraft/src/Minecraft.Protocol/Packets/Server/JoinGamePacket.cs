using Minecraft.Data.Nbt.Tags;
using Minecraft.Protocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using String = Minecraft.Protocol.Data.String;

namespace Minecraft.Protocol.Packets.Server
{
    public class JoinGamePacket : Packet
    {
        public override int PacketId => 0x26;

        public override PacketBoundTo BoundTo => PacketBoundTo.Client;

        public override ProtocolState State => ProtocolState.Play;

        /// <summary>
        /// The player's Entity ID (EID).
        /// </summary>
        public int EntityId { get; set; }

        public bool IsHardcore { get; set; }

        /// <summary>
        /// The gamemode. 
        /// </summary>
        public Gamemode Gamemode { get; set; }

        /// <summary>
        /// The previous gamemode. 
        /// </summary>
        /// <remarks>Defaults to -1 if there is no previous gamemode. (More information needed)</remarks>
        public Gamemode PreviousGamemode { get; set; }

        /// <summary>
        /// The world count.
        /// </summary>
        /// <remarks>Size of the <see cref="WorldNames"/>.</remarks>
        /// <remarks>Unnessary when send this packet.</remarks>
        public int WorldCount { get; set; }

        public NamedIdentifier[] WorldNames { get; set; }

        /// <summary>
        /// The dimension codec
        /// </summary>
        /// <remarks>The full extent of these is still unknown, but the tag represents a dimension and biome registry.</remarks>
        public NbtCompound DimensionCodec { get; set; }

        /// <summary>
        /// The dimension
        /// </summary>
        /// <remarks>Valid dimensions are defined per dimension registry sent before this. The structure of this tag is a dimension type.</remarks>
        public NbtCompound Dimension { get; set; }

        /// <summary>
        /// The world name
        /// </summary>
        /// <remarks>Name of the world being spawned into.</remarks>
        public NamedIdentifier WorldName { get; set; }

        /// <summary>
        /// The hashed seed
        /// </summary>
        /// <remarks>First 8 bytes of the SHA-256 hash of the world's seed. Used client side for biome noise.</remarks>
        public long HashedSeed { get; set; }

        /// <summary>
        /// The max players
        /// </summary>
        /// <remarks>Was once used by the client to draw the player list, but now is ignored.</remarks>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Render distance (2-32).
        /// </summary>
        public int ViewDistance { get; set; }

        /// <summary>
        /// Is debug info reduced.
        /// </summary>
        /// <remarks>If true, a Notchian client shows reduced information on the debug screen. For servers in development, this should almost always be false.</remarks>
        public bool ReducedDebugInfo { get; set; }

        /// <summary>
        /// Enable the respawn screen.
        /// </summary>
        /// <remarks>Set to false when the doImmediateRespawn gamerule is true.</remarks>
        public bool EnableRespawnScreen { get; set; }

        /// <summary>
        /// Is debug
        /// </summary>
        /// <remarks>True if the world is a debug mode world; debug mode worlds cannot be modified and have predefined blocks.</remarks>
        public bool IsDebug { get; set; }

        /// <summary>
        /// Is flat
        /// </summary>
        /// <remarks>True if the world is a superflat world; flat worlds have different void fog and a horizon at y=0 instead of y=63.</remarks>
        public bool IsFlat { get; set; }

        protected override void ReadFromStream_(ByteArray content)
        {
            EntityId = content.ReadInt();
            IsHardcore = content.ReadBoolean();
            Gamemode = (Gamemode)content.ReadUnsignedByte();
            PreviousGamemode = (Gamemode)content.ReadByte();
            WorldCount = content.ReadVarInt();
            WorldNames = content.ReadArray<String>(WorldCount).Select(s => (NamedIdentifier)(string)s).ToArray();
            DimensionCodec = content.ReadNbt<NbtCompound>();
            Dimension = content.ReadNbt<NbtCompound>();
            WorldName = content.ReadString();
            HashedSeed = content.ReadLong();
            MaxPlayers = content.ReadVarInt();
            ViewDistance = content.ReadVarInt();
            ReducedDebugInfo = content.ReadBoolean();
            EnableRespawnScreen = content.ReadBoolean();
            IsDebug = content.ReadBoolean();
            IsFlat = content.ReadBoolean();
        }

        protected override void WriteToStream_(ByteArray content)
        {
            content.Write(EntityId)
                .Write(IsHardcore)
                .Write((byte)Gamemode)
                .Write((sbyte)PreviousGamemode)
                .WriteVarInt(WorldCount)
                .WriteArray(WorldNames.Select(id => (String)(string)id).ToArray())
                .WriteNbt(DimensionCodec)
                .WriteNbt(Dimension)
                .Write(WorldName)
                .Write(HashedSeed)
                .WriteVarInt(MaxPlayers)
                .Write(ViewDistance)
                .Write(ReducedDebugInfo)
                .Write(EnableRespawnScreen)
                .Write(IsDebug)
                .Write(IsFlat);
        }

        protected override void VerifyValues()
        {
            WorldCount = WorldNames.Length;
        }
    }
}
