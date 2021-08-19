using OpenTK.Mathematics;
using static OpenTK.Mathematics.MathHelper;

namespace Minecraft.Graphics.Transforming
{
    public class Eye : IEye
    {
        private Vector2 _rotation;
        public float FovY { get; set; } = 45F;
        public float Aspect { get; set; } = 1F;
        public float DepthNear { get; set; } = .1F;
        public float DepthFar { get; set; } = 128F;
        public Vector3 Position { get; set; }

        public Vector2 Rotation
        {
            get => _rotation;
            set
            {
                value.Y = value.Y >= 0 ? Min(value.Y, 89.9F) : Max(value.Y, -89.9F);
                _rotation = value;
            }
        }
    }
}