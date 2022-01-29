using Minecraft.Numerics;

namespace Minecraft.Protocol.Client.Handlers
{
    public interface IBlockBasedRotationEntityHandler : IEntityHandler
    {
        Rotation IEntityHandler.Rotation => Rotation.FromDirection(Direction);
        Direction Direction { get; }
    }
}
