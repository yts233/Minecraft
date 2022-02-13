using Minecraft.Data.Nbt.Tags;
using OpenTK.Mathematics;
using Minecraft.Protocol.MCVersions.MC1171;
using System;

namespace Minecraft.Protocol.Client
{
    public interface IMinecraftClientAdapter
    {
        int ProtocolVersion { get; }
        
        (string locale, sbyte viewDistance, ChatMode chatMode, bool chatColors, SkinPart displayedSkinParts, HandSide mainHand, bool disableTextFiltering) ClientSettings { get; set; }
        bool IsConnected { get; }
        bool IsJoined { get; }
        bool IsLogined { get; }

        event EventHandler<(string jsonData, ChatMessagePosition position, Uuid sender)> Chat;
        event EventHandler<(string hostname, uint port)> Connected;
        event EventHandler<string> Disconnected;
        event EventHandler<(int count, int[] entityIds)> EntitiesDestroyed;
        event EventHandler<(int entityId, Vector3d delta, bool onGround)> EntityDeltaMove;
        event EventHandler<(int entityId, Vector2 rotation, bool onGround)> EntityRotation;
        event EventHandler<(int entityId, Vector3d position, Vector2 rotation, bool onGround)> EntityTeleport;
        event EventHandler<Exception> Exception;
        event EventHandler<HotbarSlot> HeldItemChange;
        event EventHandler<(int entityId, bool isHardcore, Gamemode gamemode, Gamemode previousGamemode, int worldCount, NamedIdentifier[] worldNames, NbtCompound dimensionCodec, NbtCompound dimension, NamedIdentifier worldName, long hashedSeed, int maxPlayers, int viewDistance, bool reducedDebugInfo, bool enableRespawnScreen, bool isDebug, bool isFlat)> Joined;
        event EventHandler<(string userName, Uuid uuid)> Logined;
        event EventHandler<(PlayerAbilitiy Flags, float flyingSpeed, float fieldOfViewModifier)> PlayerAbilities;
        event EventHandler<(Vector2 rotation, CoordKind yRotKind, CoordKind xRotKind, int teleportId)> PlayerLook;
        event EventHandler<(Vector3d position, CoordKind xKind, CoordKind yKind, CoordKind zKind, int teleportId, bool dismountVehicle)> PlayerPosition;
        event EventHandler<(Difficulty difficulty, bool locked)> ServerDiffculty;
        event EventHandler<(int entityId, Uuid playerUuid, Vector3d position, Vector2 rotation)> SpawnPlayer;
        event EventHandler<(int chunkX, int chunkZ)> UpdateViewPosition;

        void Connect();
        void Disconnect();
        MCVer756ProtocolAdapter GetProtocol();
        void SendChatPacket(string message);
        void SendInteractEntityPacket(int entityId, bool sneaking);
        void SendInteractEntityPacket(int entityId, Hand hand, bool sneaking);
        void SendInteractEntityPacket(int entityId, Vector3 target, Hand hand, bool sneaking);
        void SendPlayerMovementPacket(bool onGround);
        void SendPlayerPositionAndRotationPacket(Vector3d position, Vector2 rotation, bool onGround);
        void SendPlayerPositionPacket(Vector3d position, bool onGround);
        void SendPlayerRotationPacket(Vector2 rotation, bool onGround);
        void SendVehicleMovePacket(Vector3d position, Vector2 rotation);
        void SubmitClientSettings();
    }
}