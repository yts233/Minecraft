using Minecraft.Protocol.Client.Handlers;
using Minecraft.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Minecraft.Protocol.Client.Internal
{
    internal class WorldHandler : IWorldHandler
    {
        private readonly IMinecraftClientAdapter _adapter;
        private readonly IClientPlayerHandler _player;
        private readonly Dictionary<int, IEntityHandler> _entities = new Dictionary<int, IEntityHandler>();
        private readonly Dictionary<(int x, int z), IChunkHandler> _chunks = new Dictionary<(int x, int z), IChunkHandler>();

        public WorldHandler(IMinecraftClientAdapter adapter, IClientPlayerHandler player)
        {
            _adapter = adapter;
            _player = player;
            _adapter.SpawnPlayer += Adapter_SpawnPlayer;
            _adapter.EntitiesDestroyed += Adapter_EntitiesDestroyed;
            _adapter.Joined += Adapter_Joined;
        }

        private void Adapter_Joined(object sender, (int entityId, bool isHardcore, Gamemode gamemode, Gamemode previousGamemode, int worldCount, NamedIdentifier[] worldNames, Data.Nbt.Tags.NbtCompound dimensionCodec, Data.Nbt.Tags.NbtCompound dimension, NamedIdentifier worldName, long hashedSeed, int maxPlayers, int viewDistance, bool reducedDebugInfo, bool enableRespawnScreen, bool isDebug, bool isFlat) e)
        {
            //TODO: reset the world
            _entities.Clear();
            _entities.Add(e.entityId, _player);
        }

        private void Adapter_EntitiesDestroyed(object sender, (int count, int[] entityIds) e)
        {
            lock (_entities)
            {
                foreach (var id in e.entityIds)
                {
                    _entities.Remove(id);
                }
            }
        }

        private void Adapter_SpawnPlayer(object sender, (int entityId, Uuid playerUuid, Vector3d position, Rotation rotation) e)
        {
            lock (_entities)
            {
                if (!_entities.TryAdd(e.entityId, new PlayerEntityHandler(_adapter, e.entityId, e.playerUuid, e.position, e.rotation)))
                    Logger.GetLogger<WorldHandler>().Warn($"Failed to add player {e.playerUuid}, id {e.entityId}.");
            }
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

        public IChunkHandler GetChunk(int x, int z)
        {
            _chunks.TryGetValue((x, z), out var chunk);
            return chunk;
        }
    }
}
