using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Blocking
{
    internal class BlockShader : ShaderBase, IViewShader, IProjectionShader, IModelShader
    {
        private readonly int _modelPosition;
        private readonly int _viewPosition;
        private readonly int _projectionPosition;

        public BlockShader() : base(new ShaderBuilder()
            .AttachVertexShader(Shaders.BlockVertexShaderSource)
            .AttachFragmentShader(Shaders.BlockFragmentShaderSource)
            .Link())
        {
            _modelPosition = GetLocation("model");
            _viewPosition = GetLocation("view");
            _projectionPosition = GetLocation("projection");
        }

        public Matrix4 Model { get => GetMatrix4(_modelPosition); set => SetMatrix4(_modelPosition, ref value); }
        public Matrix4 View { get => GetMatrix4(_viewPosition); set => SetMatrix4(_viewPosition, ref value); }
        public Matrix4 Projection { get => GetMatrix4(_projectionPosition); set => SetMatrix4(_projectionPosition, ref value); }
    }
}