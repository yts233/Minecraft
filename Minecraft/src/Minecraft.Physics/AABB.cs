using OpenTK.Mathematics;
using System;

namespace Minecraft.Physics
{
    public struct AABB : IAABBCollisionTestable
    {
        public Box3d BoundBox { get; set; }

        public AABB(Box3d boundBox)
        {
            BoundBox = boundBox;
        }

        public void Translate(Vector3d position)
        {
            BoundBox.Translate(position);
        }

        public AABB Translated(Vector3d position)
        {
            return BoundBox.Translated(position);
        }

        public AABBHitResult CollisionTest(AABB other)
        {
            var boundThis = BoundBox;
            var boundOther = other.BoundBox;
            return new AABBHitResult(
                boundThis.Min.X < boundOther.Max.X && boundThis.Max.X > boundOther.Min.X,
                boundThis.Min.Y < boundOther.Max.Y && boundThis.Max.Y > boundOther.Min.Y,
                boundThis.Min.Z < boundOther.Max.Z && boundThis.Max.Z > boundOther.Min.Z);
        }

        public static implicit operator AABB(Box3d value)
        {
            return new AABB(value);
        }

        public static implicit operator Box3d(AABB value)
        {
            return value.BoundBox;
        }
    }
}
