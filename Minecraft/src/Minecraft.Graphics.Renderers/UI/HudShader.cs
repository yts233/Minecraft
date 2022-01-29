using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.UI
{
    internal class HudShader : ShaderBase, IViewShader, IProjectionShader, IModelShader
    {
        private readonly int _modelPosition;
        private readonly int _viewPosition;
        private readonly int _projectionPosition;

        public HudShader() : base(new ShaderBuilder()
            .AttachVertexShader(UIShaders.HudVertexShaderSource)
            .AttachFragmentShader(UIShaders.HudFragmentShaderSource)
            .Link())
        {
            Use();
            _modelPosition = GetLocation("model");
            _viewPosition = GetLocation("view");
            _projectionPosition = GetLocation("projection");
            Model = Matrix4.Identity;
            View = Matrix4.Identity;
            Projection = Matrix4.Identity;
        }

        public Matrix4 Model { get => GetMatrix4(_modelPosition); set => SetMatrix4(_modelPosition, ref value); }
        public Matrix4 View { get => GetMatrix4(_viewPosition); set => SetMatrix4(_viewPosition, ref value); }
        public Matrix4 Projection { get => GetMatrix4(_projectionPosition); set => SetMatrix4(_projectionPosition, ref value); }
    }
}
