using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public class Camera : ICamera
    {
        public Vector3 Position { get; set; }

        /// <summary>
        /// Rotation angle (in degree)
        /// </summary>
        public Vector2 Rotation { get; set; }
    }
}