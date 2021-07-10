using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Minecraft.Data.Common.Entitling;
using Minecraft.Data.Nbt;
using Minecraft.Data.Nbt.Serialization;

namespace Minecraft.Data.Common.Chunking
{
    /// <summary>
    ///     区块数据
    /// </summary>
    [NbtCompound]
    public class ChuckData
    {
        /// <summary>
        ///     该区块的X坐标。
        /// </summary>
        [NbtTag(Name = "xPos", Type = NbtTagType.Int)]
        public int X { get; set; }

        /// <summary>
        ///     该区块的Z坐标。
        /// </summary>
        [NbtTag(Name = "zPos", Type = NbtTagType.Int)]
        public int Z { get; set; }

        /// <summary>
        ///     自该区块上次保存以来经过的刻。
        /// </summary>
        [NbtTag(Name = "LastUpdate", Type = NbtTagType.Long)]
        public long LastUpdate { get; set; }

        /// <summary>
        ///     玩家已在该区块的总刻数。
        /// </summary>
        /// <remarks>注意该值在多个玩家处于同一区块时会增长得更快。</remarks>
        /// <remarks>用于区域难度：增加带有一些装备的生物的生成几率，生物自带的盔甲附有附魔效果的几率，生成带有药水效果蜘蛛的几率，生成拥有捡起物品能力的生物的几率以及决定僵尸被攻击时生成其他僵尸的几率。</remarks>
        /// <remarks>该值大于等于3600000时，区域难度会最大程度影响该区块。</remarks>
        /// <remarks>在0及以下时，难度等级被限制到最低（虽然该值设置为负数时会被设置为0，但是把该值回归到正数还是要花时间的）。</remarks>
        /// <remarks>详见区域难度获取更多信息。</remarks>
        [NbtTag(Name = "InhabitedTime", Type = NbtTagType.Long)]
        public long InhabitedTime { get; set; }

        /// <summary>
        ///     包含了1024个元素的生物群系信息。
        /// </summary>
        /// <remarks>可能不存在。</remarks>
        /// <remarks>该数组中的每一个数字表示一片4×4×4的区域的生物群系。</remarks>
        /// <remarks>这些数字以Z轴、X轴、Y轴的顺序排列。即，该数组中的前4×4个元素表示的是Y坐标为0-3之间的大小为16×16的区块的生物群系，接下来的4×4个元素表示的是Y坐标为4-7之间的生物群系，以此类推。</remarks>
        /// <remarks>生物群系ID可在Java版数据值页面查看。如果这个数组不存在，游戏在加载和保存区块时会自动根据世界的种子添加并赋值；若数组内有不对应任何生物群系的数字，游戏也会根据正确信息改正。</remarks>
        [MaybeNull]
        [NbtTag(Name = "Biomes", Type = NbtTagType.ByteArray)]
        public sbyte[] Biomes { get; set; } = new sbyte[1024];

        [NbtTag(Name = "HeightMaps", Type = NbtTagType.Compound)]
        public ChuckHeightMaps HeightMaps { get; set; } = new ChuckHeightMaps();

        [NbtTag(Name = "CarvingMasks", Type = NbtTagType.Compound)]
        public ChuckCarvingMasks CarvingMasks { get; set; } = new ChuckCarvingMasks();

        /// <summary>
        ///     一组复合标签，每个标签都包含一组子区块的描述。
        /// </summary>
        [NbtTag(Name = "Sections", Type = NbtTagType.List)]
        public ICollection<ChuckSection> Sections { get; set; } = new List<ChuckSection>();

        [NbtTag(Name = "Entities", Type = NbtTagType.List)]
        public ICollection<Entity> Entities { get; set; } = new List<Entity>();

        //TODO: https://wiki.biligame.com/mc/%E5%8C%BA%E5%9D%97%E6%A0%BC%E5%BC%8F
    }
}