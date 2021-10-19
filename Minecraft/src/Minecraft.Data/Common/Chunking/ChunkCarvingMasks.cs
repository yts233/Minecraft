using Minecraft.Data.Nbt;
using Minecraft.Data.Nbt.Serialization;

namespace Minecraft.Data.Common.Chunking
{
    [NbtCompound]
    public class ChunkCarvingMasks
    {
        [NbtTag(Name = "AIR", Type = NbtTagType.ByteArray)]
        public sbyte[] Air { get; set; }

        [NbtTag(Name = "LIQUID", Type = NbtTagType.ByteArray)]
        public sbyte[] Liquid { get; set; }
    }
}