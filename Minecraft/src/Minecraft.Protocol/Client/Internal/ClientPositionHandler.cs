using Minecraft.Protocol.Client.Handlers;
using OpenTK.Mathematics;
using System.Threading.Tasks;

namespace Minecraft.Protocol.Client.Internal
{
    internal class ClientPositionHandler : IControlablePositionHandler
    {
        private readonly IMinecraftClientAdapter _adapter;
        // 可能有性能问题，故使用field
        private Vector3d _position;
        private Vector2 _rotation;
        private bool _onGround;

        public Vector3d Position => _position;
        public Vector2 Rotation => _rotation;
        public bool OnGround => _onGround;

        public ClientPositionHandler(IMinecraftClientAdapter adapter)
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

        private void Adapter_PlayerLook(object sender, (Vector2 rotation, CoordKind yRotKind, CoordKind xRotKind, int teleportId) e)
        {
            (Vector2 rotation, CoordKind yRotKind, CoordKind xRotKind, _) = e;
            _rotation.X = xRotKind == CoordKind.Relative ? _rotation.X + rotation.X : rotation.X;
            _rotation.Y = yRotKind == CoordKind.Relative ? _rotation.Y + rotation.Y : rotation.Y;
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

        public void SetRotation(Vector2 rotation, bool onGround)
        {
            rotation.Normalize();
            _adapter.SendPlayerRotationPacket(rotation, onGround);
            _rotation = rotation;
            _onGround = onGround;
        }

        public void SetPositionAndRotation(Vector3d position, Vector2 rotation, bool onGround)
        {
            rotation.Normalize();
            _adapter.SendPlayerPositionAndRotationPacket(position, rotation, onGround);
            _position = position;
            _rotation = rotation;
            _onGround = onGround;
        }
    }
}
