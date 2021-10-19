using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    internal class ViewTransformProvider : Matrix4Provider, IViewTransformProvider
    {
        public ViewTransformProvider(ICamera camera)
        {
            Camera = camera;
        }

        public ICamera Camera { get; }

        public void CalculateMatrix()
        {
            Matrix = Matrix4.LookAt(Camera.Position, Camera.Target, Vector3.UnitY);
        }
    }
}