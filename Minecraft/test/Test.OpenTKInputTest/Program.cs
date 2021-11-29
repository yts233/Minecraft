using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Renderers.Utils;
using Minecraft.Input;
using Minecraft.Graphics.Shading;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Windowing;
using Minecraft.Graphics.Transforming;

namespace Test.OpenTKInputTest
{
    class Shader : ShaderBase
    {
        private readonly int _colorLocation;

        public Shader() : base(new ShaderBuilder()
                .AttachVertexShader(@"#version 330 core
layout (location = 0) in vec2 aPos;
out vec4 Color;

uniform vec4 color;

void main(){
    Color=color;
    gl_Position=vec4(aPos,0F,1F);
}
")
                .AttachFragmentShader(@"#version 330 core
in vec4 Color;
out vec4 FragColor;

void main(){
    FragColor=Color;
}
")
                .Link())
        {
            _colorLocation = GetLocation("color");
        }

        public Color4 Color
        {
            get => (Color4)GetVector4(_colorLocation);
            set => SetVector4(_colorLocation, (Vector4)value);
        }
    }

    class Renderer : ICompletedRenderer
    {
        private Color4 _color = Color.Aqua;
        private Shader _shader;
        private IVertexArrayHandle _circle;
        private IVertexArrayHandle _arrow;
        private readonly IAxisInput _axisInput;
        private readonly ICamera _camera;
        private readonly CameraMotivatorRenderer _cameraMotivator;

        public Renderer(IAxisInput axisInput)
        {
            _axisInput = axisInput;
            _camera = new Camera
            {
                
            };
            _cameraMotivator = new CameraMotivatorRenderer(_camera);
            _cameraMotivator.Controlable = true;
            _cameraMotivator.PositionInput = axisInput;
        }

        void IInitializer.Initialize()
        {
            GL.ClearColor(Color.Black);
            GL.LineWidth(2F);
            _shader = new Shader();
            static IEnumerable<float> CirecleVertices()
            {
                for (var i = 0; i < 360; i++)
                {
                    const double deg = Math.PI / 180D;
                    yield return (float)Math.Cos(deg * i);
                    yield return (float)Math.Sin(deg * i);
                }
            }
            static IEnumerable<VertexAttributePointer> Pointers()
            {
                yield return new()
                {
                    Index = 0,
                    Normalized = false,
                    Offset = 0,
                    Size = 2,
                    Type = VertexAttribePointerType.Float
                };
            }
            _circle = new VertexArray<float>(CirecleVertices(), Pointers()).GetHandle();
            _arrow = new VertexArray<float>(new[] { 0F, 0F, 1F, 0F }, Pointers()).GetHandle();
        }

        void IRenderable.Render()
        {
            _shader.Use();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _shader.Color = _color;
            _circle.Bind();
            _circle.Render(PrimitiveType.LineLoop);
            _arrow.Bind();
            _arrow.VertexSubData(sizeof(float) * 2, sizeof(float) * 2, new[] { _axisInput.Value.X, _axisInput.Value.Z });
            //_arrow.VertexSubData(sizeof(float) * 2, sizeof(float) * 2, new[] { _camera.Position.X, _camera.Position.Z });
            _arrow.Render(PrimitiveType.Lines);
        }

        void IUpdatable.Update()
        {
            _cameraMotivator.Update();
        }

        void ITickable.Tick()
        {
        }

        void IDisposable.Dispose()
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var window = new SimpleRenderWindowContainer
            {
                Size = (500, 500)
            };
            var axis =
            /**/
            //window.CreatePointerAxisInput();
            //axis.ZeroOnInactivate = true;
            //axis.Sensibility = 0.025F;
            window.CreateKeyAxisInput();
            axis.PositiveXKey = Keys.D;
            axis.NegativeXKey = Keys.A;
            axis.PositiveYKey = Keys.LeftShift;
            axis.NegativeYKey = Keys.Space;
            axis.PositiveZKey = Keys.W;
            axis.NegativeZKey = Keys.S;
            //axis.IsOctagon = true;
            var smoothAxis = axis.CreateSmoothAxisInput();

            var renderer = new Renderer(smoothAxis);
            window.AddRenderer(() =>
            {
                //var width = Math.Min(window.ClientSize.X, window.ClientSize.Y);
                //GL.Viewport(0, 0, width, width);
                GL.Viewport(0, 0, window.ClientSize.X, window.ClientSize.Y);
            });
            //window.AddUpdater(smoothAxis.Update);
            window.AddRenderObject(renderer);
            window.Run();
        }
    }
}
