using OpenTK.Mathematics;

namespace Minecraft.Protocol.Client.Handlers
{
    public interface IVelocityEntityHandler : IEntityHandler
    {
        Vector3d Velocity { get; }
    }
}
