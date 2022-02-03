using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.Debuggers.Axis
{
    internal sealed class AxisShader : ShaderBase, IViewShader, IProjectionShader
    {
        private readonly int _projectionLocation;
        private readonly int _viewLocation;

        public AxisShader() : base(new ShaderBuilder().AttachVertexShader(DebuggerShaders.AxisVertexShaderSource)
            .AttachFragmentShader(DebuggerShaders.AxisFragmentShaderSource).Link())
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