using Minecraft.Numerics;

namespace Minecraft.Client.Handlers
{
    internal class PlayerEntityHandler : IPlayerHandler
    {
        private readonly MinecraftClientAdapter _adapter;
        private readonly IPositionHandler _positionHandler;

        public PlayerEntityHandler(MinecraftClientAdapter adapter, int entityId, Uuid playerUuid, Vector3d position, Rotation rotation)
        {
            _adapter = adapter;
            EntityId = entityId;
            EntityUuid = playerUuid;
            _positionHandler = new EntityPositionHandler(adapter, entityId, position, rotation);
            //TODO: add events
        }

        ~PlayerEntityHandler()
        {
            //TODO: remove events
        }

        public int EntityId { get; }

        public Uuid EntityUuid { get; }

        public Vector3d Position => _positionHandler.Position;

        public Rotation Rotation => _positionHandler.Rotation;

        public bool OnGround => _positionHandler.OnGround;

        public bool IsValid { get; set; } = true;

        public IPositionHandler GetPositionHandler()
        {
            return _positionHandler;
        }
    }
}
