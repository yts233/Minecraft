using System.IO;
using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;

class TestShader : ShaderBase
{
    private readonly int _modelPosition;
    private readonly int _viewPosition;
    private readonly int _projectionPosition;

    public TestShader() : base(new ShaderBuilder().AttachVertexShader(File.ReadAllText("..\\..\\..\\vertex.glsl")).AttachFragmentShader(File.ReadAllText("..\\..\\..\\fragment.glsl")))
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