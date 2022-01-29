using Minecraft.Numerics;

namespace Minecraft.Protocol.Client.Handlers
{
    public interface IVelocityEntityHandler : IEntityHandler
    {
        Vector3d Velocity { get; }
    }
}
