using Minecraft.Numerics;
using System.Threading.Tasks;

namespace Minecraft.Client.Handlers
{
    public interface IControlablePositionHandler : IPositionHandler
    {
        Task SetMovement(bool onGround);
        Task SetPosition(Vector3d position, bool onGround);
        Task SetPositionAndRotation(Vector3d position, Rotation rotation, bool onGround);
        Task SetRotation(Rotation rotation, bool onGround);
    }
}
