using Minecraft.Client.Handlers;
using Minecraft.Numerics;
using Minecraft.Protocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minecraft.Client.Handlers
{
    internal class WorldHandler : IWorldHandler
    {
        private readonly MinecraftClientAdapter _adapter;
        private readonly IClientPlayerHandler _player;
        private readonly Dictionary<int, IEntityHandler> _entities = new();

        public WorldHandler(MinecraftClientAdapter adapter, IClientPlayerHandler player)
        {
            _adapter = adapter;
            _player = player;
            _adapter.SpawnPlayer += Adapter_SpawnPlayer;
            _adapter.EntitiesDestroyed += Adapter_EntitiesDestroyed;
            _adapter.Joined += Adapter_Joined;
        }

        private void Adapter_Joined(object sender, (int entityId, bool isHardcore, Gamemode gamemode, Gamemode previousGamemode, int worldCount, NamedIdentifier[] worldNames, Data.Nbt.Tags.NbtCompound dimensionCodec, Data.Nbt.Tags.NbtCompound dimension, NamedIdentifier worldName, long hashedSeed, int maxPlayers, int viewDistance, bool reducedDebugInfo, bool enableRespawnScreen, bool isDebug, bool isFlat) e)
        {
            _entities.Add(e.entityId, _player);
        }

        private void Adapter_EntitiesDestroyed(object sender, (int count, int[] entityIds) e)
        {
            foreach (var id in e.entityIds)
            {
                if (_entities.TryGetValue(id, out var handler))
                {
                    handler.IsValid = false;
                    _entities.Remove(id);
                }
            }
        }

        private void Adapter_SpawnPlayer(object sender, (int entityId, Uuid playerUuid, Vector3d position, Rotation rotation) e)
        {
            //for safety
            if (!_entities.TryAdd(e.entityId, new PlayerEntityHandler(_adapter, e.entityId, e.playerUuid, e.position, e.rotation)))
                Logger.GetLogger<WorldHandler>().Warn($"Failed to add player {e.playerUuid}, id {e.entityId}.");
        }

        ~WorldHandler()
        {
            _adapter.SpawnPlayer -= Adapter_SpawnPlayer;
            _adapter.EntitiesDestroyed -= Adapter_EntitiesDestroyed;
            _adapter.Joined -= Adapter_Joined;
        }

        public IReadOnlyCollection<IEntityHandler> GetEntities()
        {
            return _entities.Values;
        }
    }
}
