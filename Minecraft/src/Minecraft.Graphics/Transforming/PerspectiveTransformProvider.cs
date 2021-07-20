using OpenTK.Mathematics;
using static OpenTK.Mathematics.MathHelper;

namespace Minecraft.Graphics.Transforming
{
    public class PerspectiveTransformProvider : Matrix4Provider, IProjectionTransformProvider,
        IMatrixCalculator<Matrix4, Vector4>
    {
        public PerspectiveTransformProvider()
        {
        }

        public PerspectiveTransformProvider(IEye eye)
        {
            Eye = eye;
        }

        public IEye Eye { get; set; }

        public void CalculateMatrix()
        {
            Matrix = Matrix4.CreatePerspectiveFieldOfView(DegreesToRadians(Eye.FovY), Eye.Aspect, Eye.DepthNear,
                Eye.DepthFar);
        }
    }
}