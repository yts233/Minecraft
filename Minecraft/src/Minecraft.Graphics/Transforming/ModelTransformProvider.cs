using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public class ModelTransformProvider : MatrixProvider, IMatrixCalculator
    {
        public Vector3 Translation { get; set; } = Vector3.Zero;

        /// <summary>
        ///     Rotation Angles (in degree)
        /// </summary>
        /// <remarks>XZY</remarks>
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        public Vector3 Scale { get; set; } = Vector3.One;

        public void CalculateMatrix()
        {
            Matrix = Matrix4.CreateScale(Scale) *
                     Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X)) *
                     Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z)) *
                     Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y)) *
                     Matrix4.CreateTranslation(Translation);
        }
    }
}