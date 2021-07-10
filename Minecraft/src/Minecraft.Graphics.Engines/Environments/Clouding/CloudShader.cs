using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.Environments.Clouding
{
    public sealed class CloudShader : ShaderBase
    {
        private const string VertexShaderSource = @"#version 330 core
layout (location = 0) in vec3 aPos;
out vec4 Color;
uniform vec3 color;
uniform vec2 position;
uniform float offset;
uniform mat4 view;
uniform mat4 projection;
void main() {
    gl_Position = projection * view * vec4(aPos.x + position.x * 12, aPos.y, aPos.z + position.y * 12 - offset, 1.0F);
    Color = vec4(color, .7F);
}
";

        private const string FragmentShaderSource = @"#version 330 core
in vec4 Color;
out vec4 FragColor;

void main() {
    FragColor = Color;
}";

        private readonly int _colorLocation;
        private readonly int _positionLocation;
        private readonly int _viewLocation;
        private readonly int _projectionLocation;
        private readonly int _offsetPosition;

        public CloudShader() : base(new ShaderBuilder()
            .AttachVertexShader(VertexShaderSource)
            .AttachFragmentShader(FragmentShaderSource)
            .Link())
        {
            _colorLocation = GetLocation("color");
            _positionLocation = GetLocation("position");
            _viewLocation = GetLocation("view");
            _projectionLocation = GetLocation("projection");
            _offsetPosition = GetLocation("offset");
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

        public float Offset
        {
            get => GetFloat(_offsetPosition);
            set => SetFloat(_offsetPosition, value);
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