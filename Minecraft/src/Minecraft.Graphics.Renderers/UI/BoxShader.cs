using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.UI
{
    public class BoxShader : ShaderBase, IViewShader, IProjectionShader, IModelShader
    {
        private readonly int _viewLocation;
        private readonly int _modelLocation;
        private readonly int _projectionLocation;
        private readonly int _colorLocation;
        
        public BoxShader() : base(new ShaderBuilder().AttachFragmentShader(UIShaders.BoxFragmentShaderSource).AttachVertexShader(UIShaders.BoxVertexShaderSource))
        {
            _viewLocation = GetLocation("view");
            _modelLocation = GetLocation("model");
            _projectionLocation = GetLocation("projection");
            _colorLocation = GetLocation("color");
            Model = Matrix4.Identity;
            View = Matrix4.Identity;
            Projection = Matrix4.Identity;
        }

        public Matrix4 View { get => GetMatrix4(_viewLocation); set => SetMatrix4(_viewLocation, ref value); }
        public Matrix4 Projection { get => GetMatrix4(_projectionLocation); set => SetMatrix4(_projectionLocation, ref value); }
        public Matrix4 Model { get => GetMatrix4(_modelLocation); set => SetMatrix4(_modelLocation, ref value); }
        public Color4 Color { get => (Color4)GetVector4(_colorLocation); set => SetVector4(_colorLocation, (Vector4)value); }
    }
}
