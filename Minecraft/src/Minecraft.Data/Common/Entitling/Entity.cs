using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Minecraft.Data.Nbt;
using Minecraft.Data.Nbt.Serialization;
using OpenTK.Mathematics;

namespace Minecraft.Data.Common.Entitling
{
    /// <summary>
    /// 实体数据
    /// </summary>
    [NbtCompound]
    public class Entity
    {
        /// <summary>
        /// 实体ID。
        /// </summary>
        /// <remarks>玩家虽然也是实体，但其没有此标签。</remarks>
        [NbtTag(Name = "id", Type = NbtTagType.String)]
        public string Id { get; set; }

        /// <summary>
        /// 当前实体所剩的空气值。
        /// </summary>
        /// <remarks>在空气中充满至300，折算为生物淹没在水中15秒后生物才开始窒息，如该实体生命值为20点，那么实体在35秒后才会死亡。</remarks>
        /// <remarks>如果此项为0时实体在水中，那么其每秒会受到1点伤害。</remarks>
        [NbtTag(Name = "Air", Type = NbtTagType.Short)]
        public short Air { get; set; }

        /// <summary>
        /// 当前实体的自定义名称JSON文本组件。
        /// </summary>
        /// <remarks>可能不存在，或者存在但为空值。</remarks>
        /// <remarks>会出现在玩家的死亡信息与村民的交易界面，以及玩家的光标指向的实体的上方。</remarks>
        [MaybeNull]
        [NbtTag(Name = "CustomName", Type = NbtTagType.String)]
        public string CustomName { get; set; }

        /// <summary>
        /// 1或0（true/false），如果为true，而且实体拥有自定义名称，那么名称会总是显示在它们上方，而不受光标指向的影响。
        /// </summary>
        /// <remarks>可能不存在。</remarks>
        /// <remarks>若实体并没有自定义名称，显示的则是默认的名称。</remarks>
        [MaybeNull]
        [NbtTag(Name = "CustomNameVisible", Type = NbtTagType.Byte)]
        public byte CustomNameVisible { get; set; }

        /// <summary>
        /// 当前实体已经坠落的距离。
        /// </summary>
        /// <remarks>值越大，实体落地时对其造成伤害更多。</remarks>
        [NbtTag(Name = "FallDistance", Type = NbtTagType.Float)]
        public float FallDistance { get; set; }

        /// <summary>
        /// 距离火熄灭剩余的时间刻数。
        /// </summary>
        /// <remarks>负值表示当前实体能够在火中站立而不着火的时间。</remarks>
        /// <remarks>未着火时默认为-20。</remarks>
        [NbtTag(Name = "Fire", Type = NbtTagType.Short)]
        public short Fire { get; set; }

        /// <summary>
        /// 1或0（true/false），如果为true，那么实体会有发光的轮廓线。
        /// </summary>
        [NbtTag(Name = "Glowing", Type = NbtTagType.Byte)]
        public byte Glowing { get; set; }

        /// <summary>
        /// 1或0（true/false），如果为true，那么当前实体不会受到任何伤害。
        /// </summary>
        /// <remarks>此项对于生物与非生物实体的作用是类似的：生物不会受到任何来源（包括药水）的伤害，无法被钓鱼竿、攻击、爆炸或者抛射物推动；除支持物被移除外，诸如载具的物件都不会被摧毁。</remarks>
        /// <remarks>注意，这些实体仍然会被处于创造模式的玩家伤害到。</remarks>
        [NbtTag(Name = "Invulnerable", Type = NbtTagType.Byte)]
        public byte Invulnerable { get; set; }

        /// <summary>
        /// 记录当前实体dX、dY、dZ速度向量的3个TAG_Doubles，单位是米/每刻。
        /// </summary>
        [NbtTag(Name = "Motion", Type = NbtTagType.List)]
        public Vector3d Motion { get; set; }

        /// <summary>
        /// 1或0（true/false），如果为true，实体在空中不会坠落，在盔甲架上的效果却是使 Motion标签将失去效果（标签数据依旧存在，且持续被系统运算）。
        /// </summary>
        [NbtTag(Name = "NoGravity", Type = NbtTagType.Byte)]
        public byte NoGravity { get; set; }

        /// <summary>
        /// 1或0（true/false），实体接触地面时为true。
        /// </summary>
        [NbtTag(Name = "OnGround", Type = NbtTagType.Byte)]
        public byte OnGround { get; set; }

        /// <summary>
        /// 正在骑乘当前实体的实体的数据。
        /// </summary>
        /// <remarks>注意，两个实体都能控制移动，被刷怪笼召唤时生成条件由最上方的实体决定。 </remarks>
        public ICollection<Entity> Passengers { get; set; }

        /// <summary>
        /// 距离当前实体可以再次穿过下界传送门向回传送的时间刻数。
        /// </summary>
        /// <remarks>在初次传送后，起始值为300刻（15秒）并逐渐倒计时至0。</remarks>
        public int PortalCooldown { get; set; }
    }
}