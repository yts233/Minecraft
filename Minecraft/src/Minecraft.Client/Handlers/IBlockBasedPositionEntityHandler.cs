using Minecraft.Numerics;

namespace Minecraft.Client.Handlers
{
    public interface IBlockBasedPositionEntityHandler : IEntityHandler
    {
        Vector3d IEntityHandler.Position => (Vector3d)Position;
        new Vector3i Position { get; }
    }
}
