using Minecraft.Client.Handlers;
using Minecraft.Numerics;
using System.Threading.Tasks;

namespace Minecraft.Client.Internal
{
    internal class ClientPositionHandler : IControlablePositionHandler
    {
        private readonly MinecraftClientAdapter _adapter;
        // 可能有性能问题，故使用field
        private Vector3d _position;
        private Rotation _rotation;
        private bool _onGround;

        public Vector3d Position => _position;
        public Rotation Rotation => _rotation;
        public bool OnGround => _onGround;

        public ClientPositionHandler(MinecraftClientAdapter adapter)
        {
            _adapter = adapter;
            _adapter.PlayerPosition += Adapter_PlayerPosition;
            _adapter.PlayerLook += Adapter_PlayerLook;
        }

        ~ClientPositionHandler()
        {
            _adapter.PlayerPosition -= Adapter_PlayerPosition;
            _adapter.PlayerLook -= Adapter_PlayerLook;
        }

        private void Adapter_PlayerPosition(object sender, (Vector3d position, CoordKind xKind, CoordKind yKind, CoordKind zKind, int teleportId, bool dismountVehicle) e)
        {
            (Vector3d position, CoordKind xKind, CoordKind yKind, CoordKind zKind, _, _) = e;
            _position.X = xKind == CoordKind.Relative ? _position.X + position.X : position.X;
            _position.Y = yKind == CoordKind.Relative ? _position.Y + position.Y : position.Y;
            _position.Z = zKind == CoordKind.Relative ? _position.Z + position.Z : position.Z;
        }

        private void Adapter_PlayerLook(object sender, (Rotation rotation, CoordKind yRotKind, CoordKind xRotKind, int teleportId) e)
        {
            (Rotation rotation, CoordKind yRotKind, CoordKind xRotKind, _) = e;
            _rotation.Yaw = xRotKind == CoordKind.Relative ? _rotation.Yaw + rotation.Yaw : rotation.Yaw;
            _rotation.Pitch = yRotKind == CoordKind.Relative ? _rotation.Pitch + rotation.Pitch : rotation.Pitch;
            _rotation.Normalize();
        }

        public void SetMovement(bool onGround)
        {
            _adapter.SendPlayerMovementPacket(onGround);
            _onGround = onGround;
        }

        public void SetPosition(Vector3d position, bool onGround)
        {
            _adapter.SendPlayerPositionPacket(position, onGround);
            _position = position;
            _onGround = onGround;
        }

        public void SetRotation(Rotation rotation, bool onGround)
        {
            rotation.Normalize();
            _adapter.SendPlayerRotationPacket(rotation, onGround);
            _rotation = rotation;
            _onGround = onGround;
        }

        public void SetPositionAndRotation(Vector3d position, Rotation rotation, bool onGround)
        {
            rotation.Normalize();
            _adapter.SendPlayerPositionAndRotationPacket(position, rotation, onGround);
            _position = position;
            _rotation = rotation;
            _onGround = onGround;
        }
    }
}
