using System.Collections.Generic;
using Minecraft.Data.Common.Blocking;
using Minecraft.Data.Nbt;
using Minecraft.Data.Nbt.Serialization;

namespace Minecraft.Data.Common.Chunking
{
    /// <summary>
    ///     一个独立的子区块。
    /// </summary>
    [NbtCompound]
    public class ChuckSection
    {
        /// <summary>
        ///     这个子区块的Y索引（不是坐标）。
        /// </summary>
        /// <remarks>范围为0到15（从低到高），不会重复，但对应子区块为空时可能会没有。</remarks>
        [NbtTag(Name = "Y", Type = NbtTagType.Byte)]
        public sbyte Y { get; set; }

        /// <summary>
        ///     几组在区块中用到的方块状态。
        /// </summary>
        [NbtTag(Name = "Palette", Type = NbtTagType.List)]
        public IList<BlockState> Palette { get; set; } = new List<BlockState> {new BlockState("air")};

        /// <summary>
        ///     2048字节记录了每个方块发出的亮度。
        /// </summary>
        /// <remarks>使加载时间相较于重新计算变得更快。</remarks>
        /// <remarks>每方块占用4字节。</remarks>
        [NbtTag(Name = "BlockLight", Type = NbtTagType.ByteArray)]
        public sbyte[] BlockLight { get; set; } = new sbyte[2048];

        /// <summary>
        ///     一个64位长的变量，能够存储4096个索引的数组。
        /// </summary>
        /// <remarks>索引的顺序对应Sections中Palette元素的顺序。</remarks>
        /// <remarks>每方块占用4字节。</remarks>
        /// <remarks>在Section中的所有索引大小相同，其大小需要表示最大的索引（最小为4位）。</remarks>
        /// <remarks>若此索引不能被64整除，其位数会继续增加。</remarks>
        [NbtTag(Name = "BlockStates", Type = NbtTagType.LongArray)]
        public long[] BlockStates { get; set; } = new long[4096];

        /// <summary>
        ///     2048字节记录了阳光和月光打在每个方块上的亮度。
        /// </summary>
        /// <remarks>每方块占用4字节。</remarks>
        [NbtTag(Name = "SkyLight", Type = NbtTagType.ByteArray)]
        public sbyte[] SkyLight { get; set; } = new sbyte[2048];
    }
}