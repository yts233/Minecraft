using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public interface IModel
    {
        Vector3 Translation { get; set; }

        /// <summary>
        /// Rotation Angles (in degree)
        /// </summary>
        Vector3 Rotation { get; set; }

        Vector3 Scale { get; set; }
    }
}