namespace Minecraft.Physics
{
    public interface IAABBCollisionTestable
    {
        /// <summary>
        /// 碰撞测试
        /// </summary>
        /// <param name="other">另一个碰撞箱</param>
        /// <returns>重合的轴</returns>
        AABBHitResult CollisionTest(AABB other);
    }
}
