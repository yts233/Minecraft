using Minecraft.Data;
using OpenTK.Mathematics;
using System;

namespace Minecraft.Physics
{
    public interface IBlockCollisionObject : IAABBCollisionTestable
    {
        IBlockProvider BlockProvider { get; }

        AABBHitResult IAABBCollisionTestable.CollisionTest(AABB other)
        {
            var box = other.BoundBox;
            int mmx = (int)Math.Floor(box.Min.X),
                mmy = (int)Math.Floor(box.Min.Y),
                mmz = (int)Math.Floor(box.Min.Z),
                mxx = (int)Math.Floor(box.Max.X),
                mxy = (int)Math.Floor(box.Max.Y),
                mxz = (int)Math.Floor(box.Max.Z);

            for (int x = mmx; x <= mxx; x++)
            {
                for (int y = mmy; y <= mxy; y++)
                {
                    //if (x == mmx || x == mxx || y == mmy || y == mxy)
                    //{
                    for (int z = mmz; z <= mxz; z++)
                    {
                        if (BlockProvider.IsTile(x, y, z))
                            return new AABBHitResult(0b111);
                    }
                    //}
                    //else
                    //{
                    //    if (BlockProvider.IsTile(x, y, mmz) || BlockProvider.IsTile(x, y, mxz))
                    //        return true;
                    //}
                }
            }
            return new AABBHitResult(0b000);
        }

        RayCastResult CollisionTest(Ray other)
        {
            return default;
        }
    }

    public struct Ray
    {
        public Vector3d Position;
        public Vector3d Direction;
        public double Range;

        RayCastResult RayCast(AABB other)
        {
            return default;
        }
    }

    public struct RayCastResult
    {
        public AABB AABB;
        public Ray Ray;
        public Vector3d HitFace;
        public Vector2d HitPosition;
        public bool IsCollision => HitFace == Vector3d.Zero;
    }
}
