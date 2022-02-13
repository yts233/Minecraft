namespace Minecraft.Physics
{
    public interface ICollisionAABBObject : IAABBCollisionTestable
    {
        AABB OriginalAABB { get; set; }
        AABB TranslatedAABBB { get; set; }

        AABBHitResult IAABBCollisionTestable.CollisionTest(AABB other)
        {
            return TranslatedAABBB.CollisionTest(other);
        }

        AABBHitResult CollisionTest(ICollisionAABBObject other)
        {
            return TranslatedAABBB.CollisionTest(other.TranslatedAABBB);
        }
    }
}
