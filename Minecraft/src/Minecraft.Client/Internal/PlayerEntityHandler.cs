using Minecraft.Client.Handlers;
using Minecraft.Numerics;
using System.Linq;

namespace Minecraft.Client.Internal
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
            _adapter.EntitiesDestroyed += Adapter_EntitiesDestroyed;
        }

        private void Adapter_EntitiesDestroyed(object sender, (int count, int[] entityIds) e)
        {
            if (e.entityIds.Contains(EntityId))
            {
                IsValid = false;
            }
        }

        ~PlayerEntityHandler()
        {
            //TODO: remove events
            _adapter.EntitiesDestroyed -= Adapter_EntitiesDestroyed;
        }

        public int EntityId { get; }

        public Uuid EntityUuid { get; }

        public Vector3d Position => _positionHandler.Position;

        public Rotation Rotation => _positionHandler.Rotation;

        public bool OnGround => _positionHandler.OnGround;

        public bool IsValid { get; private set; } = true;

        public IPositionHandler GetPositionHandler()
        {
            return _positionHandler;
        }
    }
}
