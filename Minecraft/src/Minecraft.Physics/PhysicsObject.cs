using Minecraft.Data;
using OpenTK.Mathematics;

namespace Minecraft.Physics
{
    public class BlockCollisionObject : IBlockCollisionObject
    {
        public IBlockProvider BlockProvider { get; }

        public BlockCollisionObject(IBlockProvider blockProvider)
        {
            BlockProvider = blockProvider;
        }
    }

    public class PhysicsObject : IPhysicsObject, ICollisionAABBObject
    {
        private Vector3d _position;
        private Vector3d _velocity;
        private double _gravityScale = 1D;
        private double _mass = 1D;
        private AABB _aabb;

        public Vector3d Position { get => _position; set => _position = value; }
        public Vector3d Velocity { get => _velocity; set => _velocity = value; }
        public double GravityScale { get => _gravityScale; set => _gravityScale = value; }
        public double Mass { get => _mass; set => _mass = value; }
        public AABB OriginalAABB { get => _aabb; set => _aabb = value; }
        public AABB TranslatedAABBB { get => _aabb.Translated(Position); set => _aabb = value.Translated(-Position); }

        public void Update()
        {
            _velocity.Y -= _gravityScale / 6D;
            _position += _velocity / 60D;
        }
    }
}
