using Minecraft.Numerics;

namespace Minecraft.Client.Handlers
{
    public interface IVelocityEntityHandler : IEntityHandler
    {
        Vector3d Velocity { get; }
    }
}
