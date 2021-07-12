using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.Debuggers.Axis
{
    public sealed class AxisShader : ShaderBase, IViewShader, IProjectionShader
    {
        private readonly int _projectionLocation;
        private readonly int _viewLocation;

        private const string VertexShaderSource = @"#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;
out vec4 color;
uniform mat4 view;
uniform mat4 projection;

void main() {
    gl_Position = projection * view * vec4(aPos, 1.0F);
    color = vec4(aColor, 1.0F);
}
";

        private const string FragmentShaderSource = @"#version 330 core
in vec4 color;
out vec4 FragColor;

void main() {
    FragColor = color;
}";

        public AxisShader() : base(new ShaderBuilder().AttachVertexShader(VertexShaderSource)
            .AttachFragmentShader(FragmentShaderSource).Link())
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