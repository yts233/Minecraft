using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.Environments.Clouding
{
    public sealed class CloudShader : ShaderBase, IProjectionShader, IViewShader
    {
        private const string VertexShaderSource = @"#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
out vec3 objectColor;
out vec3 normal;
uniform vec3 color;
uniform vec2 position;
uniform float offset;
uniform mat4 view;
uniform mat4 projection;
void main() {
    gl_Position = projection * view * vec4(aPos.x * 12 + position.x * 12, aPos.y * 4 + 128, aPos.z * 12 + position.y * 12 - offset, 1.0F);
    objectColor = color;
    normal = aNormal;
}
";

        private const string FragmentShaderSource = @"#version 330 core
in vec3 objectColor;
in vec3 normal;
out vec4 FragColor;

void main() {
    vec3 lightColor = vec3(1F);
    vec3 ambient = 0.8F * lightColor;
    float diffA = max(dot(normal, normalize(vec3(1.0F,5.0F,1.0F))), 0.0);
    float diffB = max(dot(normal, normalize(vec3(-2.0F,1.0F,-1.0F))), 0.0);
    vec3 diffuseA = diffA * lightColor * .7F;
    vec3 diffuseB = diffB * lightColor * .1F;
    vec3 result = (ambient + diffuseA + diffuseB) * objectColor;
    FragColor = vec4(result, 1F);
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