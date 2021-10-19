using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;

namespace Test.OpenGL.ChunkRenderTest
{
    class SimpleShader : ShaderBase
    {
        private readonly int _viewLocation;
        private readonly int _projectionLocation;

        public SimpleShader() : base(new ShaderBuilder()
            .AttachVertexShader(Shaders.SimpleVertexShaderSource)
            .AttachFragmentShader(Shaders.SimpleFragmentShaderSource)
            .Link())
        {
            _viewLocation = GetLocation("view");
            _projectionLocation = GetLocation("projection");
        }

        public Matrix4 View
        {
            get => GetMatrix4(_viewLocation);
            set => SetMatrix4(_viewLocation, ref value);
        }

        public Matrix4 Projection
        {
            get => GetMatrix4(_projectionLocation);
            set => SetMatrix4(_projectionLocation, ref value);
        }
    }
}
