using Minecraft.Graphics.Shading;
using Minecraft.Resources;
using OpenTK.Mathematics;

namespace Test.OpenGL.Test
{
    public class TestShader : ShaderBase
    {
        private static int _modelLocation, _viewLocation, _projectionLocation;
        private static readonly IFilePath _filePath = new FilePath();

        public TestShader() : base(new ShaderBuilder()
            .AttachVertexShader(_filePath["vertexShader.glsl"].ReadAllText())
            .AttachFragmentShader(_filePath["fragmentShader.glsl"].ReadAllText())
            .Link())
        {
            Use();
            _modelLocation = GetLocation("model");
            _viewLocation = GetLocation("view");
            _projectionLocation = GetLocation("projection");
            Model = Matrix4.Identity;
            View = Matrix4.Identity;
            Projection = Matrix4.Identity;
        }

        public Matrix4 Model
        {
            get => GetMatrix4(_modelLocation);
            set => SetMatrix4(_modelLocation, ref value);
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