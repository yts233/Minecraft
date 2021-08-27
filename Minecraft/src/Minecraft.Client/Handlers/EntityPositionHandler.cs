using Minecraft.Numerics;

namespace Minecraft.Client.Handlers
{
    internal class EntityPositionHandler : IPositionHandler
    {
        private readonly MinecraftClientAdapter _adapter;
        private readonly int _entityId;
        private Vector3d _position;
        private Rotation _rotation;
        private bool _onGround;

        public EntityPositionHandler(MinecraftClientAdapter adapter, int entityId, Vector3d position, Rotation rotation)
        {
            _adapter = adapter;
            _entityId = entityId;
            _position = position;
            _rotation = rotation;
            _adapter.EntityDeltaMove += Adapter_EntityDeltaMove;
            _adapter.EntityRotation += Adapter_EntityRotation;
            _adapter.EntityTeleport += Adapter_EntityTeleport;
        }

        private void Adapter_EntityTeleport(object sender, (int entityId, Vector3d position, Rotation rotation, bool onGround) e)
        {
            if (e.entityId != _entityId) return;
            _position = e.position;
            _rotation = e.rotation;
            _onGround = OnGround;
        }

        private void Adapter_EntityRotation(object sender, (int entityId, Rotation rotation, bool onGround) e)
        {
            if (e.entityId != _entityId) return;
            _rotation = e.rotation;
            _onGround = OnGround;
        }

        private void Adapter_EntityDeltaMove(object sender, (int entityId, Vector3d delta, bool onGround) e)
        {
            if (e.entityId != _entityId) return;
            _position.Add(e.delta);
            _onGround = OnGround;
        }

        ~EntityPositionHandler()
        {
            _adapter.EntityDeltaMove -= Adapter_EntityDeltaMove;
            _adapter.EntityRotation -= Adapter_EntityRotation;
            _adapter.EntityTeleport -= Adapter_EntityTeleport;
        }

        public Vector3d Position => _position;

        public Rotation Rotation => _rotation;

        public bool OnGround => _onGround;
    }
}
