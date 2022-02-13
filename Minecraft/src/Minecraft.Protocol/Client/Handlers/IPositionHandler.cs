using OpenTK.Mathematics;

namespace Minecraft.Protocol.Client.Handlers
{
    public interface IPositionHandler
    {
        public Vector3d Position { get; }
        public Vector2 Rotation { get; }
        bool OnGround { get; }
    }
}
