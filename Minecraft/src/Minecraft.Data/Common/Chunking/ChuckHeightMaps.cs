using Minecraft.Data.Nbt;
using Minecraft.Data.Nbt.Serialization;

namespace Minecraft.Data.Common.Chunking
{
    /// <summary>
    ///     几个不同的高度图。
    /// </summary>
    /// <remarks>每个高度图都储存了被储存为9位（即取值范围为[0,256]）的256个数值，来表示相应的高度。 </remarks>
    [NbtCompound]
    public class ChuckHeightMaps
    {
        /// <summary>
        ///     最高的能阻挡移动或含有液体的方块。
        /// </summary>
        [NbtTag(Name = "MOTION_BLOCKING", Type = NbtTagType.LongArray)]
        public long[] MotionBlocking { get; set; }

        /// <summary>
        ///     最高的阻挡移动、含有液体或在minecraft:leaves标签里的方块。
        /// </summary>
        [NbtTag(Name = "MOTION_BLOCKING_NO_LEAVES", Type = NbtTagType.LongArray)]
        public long[] MotionBlockingNoLeaves { get; set; }

        /// <summary>
        ///     最高的非空气固体方块。
        /// </summary>
        [NbtTag(Name = "OCEAN_FLOOR", Type = NbtTagType.LongArray)]
        public long[] OceanFloor { get; set; }

        /// <summary>
        ///     最高的既不是空气也不包含液体的方块。
        /// </summary>
        /// <remarks>此值用于世界生成。</remarks>
        [NbtTag(Name = "OCEAN_FLOOR_WG", Type = NbtTagType.LongArray)]
        public long[] OceanFloorWg { get; set; }

        /// <summary>
        ///     最高的非空气方块。
        /// </summary>
        [NbtTag(Name = "WORLD_SURFACE", Type = NbtTagType.LongArray)]
        public long[] WorldSurface { get; set; }

        /// <summary>
        ///     最高的非空气方块。
        /// </summary>
        /// <remarks>此值用于世界生成。</remarks>
        [NbtTag(Name = "WORLD_SURFACE_WG", Type = NbtTagType.LongArray)]
        public long[] WorldSurfaceWg { get; set; }
    }
}