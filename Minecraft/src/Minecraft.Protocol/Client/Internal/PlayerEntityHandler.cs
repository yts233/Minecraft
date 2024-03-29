﻿using Minecraft.Protocol.Client.Handlers;
using OpenTK.Mathematics;
using System.Linq;

namespace Minecraft.Protocol.Client.Internal
{
    internal class PlayerEntityHandler : IPlayerHandler
    {
        private readonly IMinecraftClientAdapter _adapter;
        private readonly IPositionHandler _positionHandler;

        public PlayerEntityHandler(IMinecraftClientAdapter adapter, int entityId, Uuid playerUuid, Vector3d position, Vector2 rotation)
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

        public Vector2 Rotation => _positionHandler.Rotation;

        public bool OnGround => _positionHandler.OnGround;

        public bool IsValid { get; private set; } = true;

        public IPositionHandler GetPositionHandler()
        {
            return _positionHandler;
        }
    }
}
