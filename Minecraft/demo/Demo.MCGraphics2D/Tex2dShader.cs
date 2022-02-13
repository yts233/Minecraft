using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Shading;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Demo.MCGraphics2D
{
    public class Tex2dShader : ShaderBase
    {
        private static readonly string VertexShaderSource = @"#version 330 core
layout(location = 0) in vec2 aPos;
layout(location = 1) in vec2 aTex;
out vec2 TexCoord;
out vec4 Color;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec4 color;
uniform float depth;

void main(){
    gl_Position = projection * vec4(vec2(view * model * vec4(aPos,0F,1F)), depth,1F);
    Color=color;
    TexCoord=aTex;
}
";
        private static readonly string FragmentShaderSource = @"#version 330 core
in vec2 TexCoord;
in vec4 Color;
out vec4 FragColor;
uniform sampler2D texture1;

void main(){
    vec4 outColor = texture(texture1,TexCoord)*Color;
    if(outColor.w<0.01F)
        discard;
    FragColor=outColor;
}";
        private readonly int _projectionLocation;
        private readonly int _colorLocation;
        private readonly int _depthLocation;
        private readonly int _modelLocation;
        private readonly int _viewLocation;

        public Tex2dShader() : base(new ShaderBuilder().AttachVertexShader(VertexShaderSource).AttachFragmentShader(FragmentShaderSource))
        {
            _modelLocation = GetLocation("model");
            _viewLocation = GetLocation("view");
            _projectionLocation = GetLocation("projection");
            _colorLocation = GetLocation("color");
            _depthLocation = GetLocation("depth");
            Color = Color4.White;
            Model = Matrix4.Identity;
            View = Matrix4.Identity;
            Projection = Matrix4.Identity;
            Depth = 0F;
        }

        public Color4 Color { get => (Color4)GetVector4(_colorLocation); set => SetVector4(_colorLocation, (Vector4)value); }
        public Matrix4 Model { get => GetMatrix4(_modelLocation); set => SetMatrix4(_modelLocation, ref value); }
        public Matrix4 View { get => GetMatrix4(_viewLocation); set => SetMatrix4(_viewLocation, ref value); }
        public Matrix4 Projection { get => GetMatrix4(_projectionLocation); set => SetMatrix4(_projectionLocation, ref value); }
        public float Depth { get => GetFloat(_depthLocation); set => SetFloat(_depthLocation, value); }

        public static IEnumerable<VertexAttributePointer> GetPointers()
        {
            yield return new VertexAttributePointer
            {
                Index = 0,
                Normalized = false,
                Offset = 0,
                Size = 2,
                Type = VertexAttribePointerType.Float
            };
            yield return new VertexAttributePointer
            {
                Index = 1,
                Normalized = false,
                Offset = 2 * sizeof(float),
                Size = 2,
                Type = VertexAttribePointerType.Float
            };
        }
    }
}
