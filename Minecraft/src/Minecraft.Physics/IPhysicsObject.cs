using OpenTK.Mathematics;
using System;

namespace Minecraft.Physics
{
    /// <summary>
    /// 一个物体
    /// </summary>
    public interface IPhysicsObject : IUpdatable
    {
        /// <summary>
        /// 坐标
        /// </summary>
        Vector3d Position { get; set; }
        /// <summary>
        /// 每秒速度
        /// </summary>
        Vector3d Velocity { get; set; }
        /// <summary>
        /// 重力
        /// </summary>
        double GravityScale { get; set; }
        /// <summary>
        /// 质量
        /// </summary>
        double Mass { get; set; }
    }
}
