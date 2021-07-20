using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public class Model : IModel
    {
        public Vector3 Translation { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }
}