using Minecraft.Data.Nbt;
using Minecraft.Data.Nbt.Serialization;

namespace Minecraft.Data.Common.Chunking
{
    /// <summary>
    /// 区块
    /// </summary>
    [NbtCompound]
    public class Chuck
    {
        /// <summary>
        /// 该区块NBT结构的版本。
        /// </summary>
        [NbtTag(Name = "DataVersion", Type = NbtTagType.Int)]
        public int Version { get; set; }

        /// <summary>
        /// 区块数据。
        /// </summary>
        [NbtTag(Name = "Level", Type = NbtTagType.Compound)]
        public ChuckData Data { get; set; }
    }
}