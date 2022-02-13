using OpenTK.Mathematics;
using System.Threading.Tasks;

namespace Minecraft.Protocol.Client.Handlers
{
    public interface IControlablePositionHandler : IPositionHandler
    {
        void SetMovement(bool onGround);
        void SetPosition(Vector3d position, bool onGround);
        void SetPositionAndRotation(Vector3d position, Vector2 rotation, bool onGround);
        void SetRotation(Vector2 rotation, bool onGround);
    }
}
