using Minecraft.Numerics;

namespace Minecraft.Client.Handlers
{
    public interface IPositionHandler
    {
        public Vector3d Position { get; }
        public Rotation Rotation { get; }
        bool OnGround { get; }
    }
}
