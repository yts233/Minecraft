using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.Environments.Clouding
{
    internal sealed class CloudShader : ShaderBase, IProjectionShader, IViewShader
    {
        private readonly int _colorLocation;
        private readonly int _positionLocation;
        private readonly int _viewLocation;
        private readonly int _projectionLocation;
        private readonly int _offsetLocation;
        private readonly int _centerPositionLocation;

        public CloudShader() : base(new ShaderBuilder()
            .AttachVertexShader(EnvironmentShaders.CloudVertexShaderSource)
            .AttachFragmentShader(EnvironmentShaders.CloudFragmentShaderSource)
            .Link())
        {
            _colorLocation = GetLocation("color");
            _positionLocation = GetLocation("position");
            _viewLocation = GetLocation("view");
            _projectionLocation = GetLocation("projection");
            _offsetLocation = GetLocation("offset");
            _centerPositionLocation = GetLocation("centerPosition");
        }

        public Vector3 Color
        {
            get => GetVector3(_colorLocation);
            set => SetVector3(_colorLocation, value);
        }

        public Vector2 Position
        {
            get => GetVector2(_positionLocation);
            set => SetVector2(_positionLocation, value);
        }

        public Vector3 CenterPosition
        {
            get => GetVector3(_centerPositionLocation);
            set => SetVector3(_centerPositionLocation, value);
        }

        public float Offset
        {
            get => GetFloat(_offsetLocation);
            set => SetFloat(_offsetLocation, value);
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