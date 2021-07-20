using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public class ViewTransformProvider : Matrix4Provider, IMatrixCalculator<Matrix4,Vector4>
    {
        public ViewTransformProvider()
        {
        }

        public ViewTransformProvider(ICamera camera)
        {
            Camera = camera;
        }

        public ICamera Camera { get; set; }

        public void CalculateMatrix()
        {
            Matrix = Matrix4.LookAt(Camera.Position, Camera.Target, Vector3.UnitY);
        }
    }
}