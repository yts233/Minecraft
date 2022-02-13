using Minecraft.Extensions;
using OpenTK.Mathematics;

namespace Minecraft.Protocol.Client.Handlers
{
    public interface IBlockBasedRotationEntityHandler : IEntityHandler
    {
        Vector2 IEntityHandler.Rotation => Direction.ToRotation();
        Direction Direction { get; }
    }
}
