using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Minecraft.Resources;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Renderers.Environments.Clouding;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Shading;
using Minecraft.Graphics.Transforming;
using Minecraft.Graphics.Windowing;
using Minecraft.Input;
using Minecraft.Resources.Vanilla.WorldOfColorUpdate;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Test.OpenGL.Test0
{
    class Program
    {

        private class Shader : ShaderBase
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

        private class Renderer : ICompletedRenderer
        {
            private Color4 _color = Color.Red;
            private Shader _shader;
            private IVertexArrayHandle _circle;
            private IVertexArrayHandle _arrow;
            private Vector3d _rawDelta;
            private Vector3d _delta;

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
                _circle.Render(OpenTK.Graphics.OpenGL4.PrimitiveType.LineLoop);
                _arrow.Bind();
                _arrow.VertexSubData(sizeof(float) * 2, sizeof(float) * 2, new[] { (float)_delta.X, (float)_delta.Z });
                _arrow.Render(OpenTK.Graphics.OpenGL4.PrimitiveType.Lines);
            }

            void IUpdatable.Update()
            {
                Vector3d target;
                if (_rawDelta.LengthSquared < 0.001D)
                    target = Vector3d.Zero;
                else target = _rawDelta.Normalized();
                var d = target - _delta;
                if (d.LengthSquared < 0.001D)
                {
                    _delta = target;
                    return;
                }
                _delta += d * .1D;
            }

            public void OnKeyDown(KeyboardKeyEventArgs e)
            {
                switch (e.Key)
                {
                    case Keys.Up:
                        _rawDelta.Z = 1D;
                        break;
                    case Keys.Down:
                        _rawDelta.Z = -1D;
                        break;
                    case Keys.Left:
                        _rawDelta.X = -1D;
                        break;
                    case Keys.Right:
                        _rawDelta.X = 1D;
                        break;
                }
            }

            public void OnKeyUp(KeyboardKeyEventArgs e)
            {
                switch (e.Key)
                {
                    case Keys.Up:
                        if (Math.Abs(_rawDelta.Z - 1D) < 0.1D)
                            _rawDelta.Z = 0D;
                        break;
                    case Keys.Down:
                        if (Math.Abs(_rawDelta.Z + 1D) < 0.1D)
                            _rawDelta.Z = 0D;
                        break;
                    case Keys.Left:
                        if (Math.Abs(_rawDelta.X + 1D) < 0.1D)
                            _rawDelta.X = 0D;
                        break;
                    case Keys.Right:
                        if (Math.Abs(_rawDelta.X - 1D) < 0.1D)
                            _rawDelta.X = 0D;
                        break;
                }
            }

            void ITickable.Tick()
            {
            }

            void IDisposable.Dispose()
            {

            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var window = new RenderWindow
            {
                Size = (200, 200)
            };
            var renderer = new Renderer();
            window.AddRenderer(() =>
            {
                var width = Math.Min(window.ClientSize.X, window.ClientSize.Y);
                GL.Viewport(0, 0, width, width);
            });
            window.AddObject(renderer);
            window.KeyDown += (_, obj) => renderer.OnKeyDown(obj);
            window.KeyUp += (_, obj) => renderer.OnKeyUp(obj);
            window.ReloadWindow();
        }

    }
}
