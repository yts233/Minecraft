using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public class ModelTransformProvider : Matrix4Provider, IMatrixCalculator<Matrix4, Vector4>
    {
        public IModel Model { get; set; }

        public void CalculateMatrix()
        {
            Matrix = Matrix4.CreateScale(Model.Scale) *
                     Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Model.Rotation.X)) *
                     Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Model.Rotation.Z)) *
                     Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Model.Rotation.Y)) *
                     Matrix4.CreateTranslation(Model.Translation);
        }
    }
}