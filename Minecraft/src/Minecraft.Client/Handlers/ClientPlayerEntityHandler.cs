using Minecraft.Numerics;

namespace Minecraft.Client.Handlers
{
    internal class ClientPlayerEntityHandler : IClientPlayerHandler
    {
        private readonly MinecraftClientAdapter _adapter;
        private readonly IControlablePositionHandler _positionHandler;
        private Uuid _uuid;
        private int _entityId;

        public ClientPlayerEntityHandler(MinecraftClientAdapter adapter)
        {
            _adapter = adapter;
            _positionHandler = new ClientPositionHandler(_adapter);
            _adapter.Logined += Adapter_Logined;
            _adapter.Joined += Adapter_Joined;
            _adapter.Disconnected += Adapter_Disconnected;
        }

        private void Adapter_Disconnected(object sender, string e)
        {
            IsValid = false;
        }

        private void Adapter_Joined(object sender, (int entityId, bool isHardcore, Gamemode gamemode, Gamemode previousGamemode, int worldCount, NamedIdentifier[] worldNames, Data.Nbt.Tags.NbtCompound dimensionCodec, Data.Nbt.Tags.NbtCompound dimension, NamedIdentifier worldName, long hashedSeed, int maxPlayers, int viewDistance, bool reducedDebugInfo, bool enableRespawnScreen, bool isDebug, bool isFlat) e)
        {
            // the player is valid now;
            IsValid = true;
            _entityId = e.entityId;
        }

        private void Adapter_Logined(object sender, (string userName, Uuid uuid) e)
        {
            _uuid = e.uuid;
        }

        ~ClientPlayerEntityHandler()
        {
            _adapter.Logined -= Adapter_Logined;
            _adapter.Joined -= Adapter_Joined;
            _adapter.Disconnected -= Adapter_Disconnected;
        }

        public int EntityId => _entityId;

        public Uuid EntityUuid => _uuid;

        public Vector3d Position => _positionHandler.Position;

        public Rotation Rotation => _positionHandler.Rotation;

        public bool OnGround => _positionHandler.OnGround;

        public bool IsValid { get; set; } = false;

        public IControlablePositionHandler GetPositionHandler()
        {
            return _positionHandler;
        }
    }
}
