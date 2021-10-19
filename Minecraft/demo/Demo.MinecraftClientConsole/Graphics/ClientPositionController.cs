using Minecraft.Client;
using Minecraft.Client.Handlers;
using Minecraft.Input;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Windowing;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minecraft.Graphics.Shading;
using Minecraft.Graphics.Arraying;
using System.Timers;

namespace Demo.MinecraftClientConsole.Graphics
{
    public class ClientPositionController
    {
        private readonly MinecraftClient _client;
        private readonly Renderer _renderer;
        private readonly RenderWindow _window;

        public ClientPositionController(MinecraftClient client)
        {
            _client = client;
            _window = new RenderWindow
            {
                Size = (200, 200)
            };
            _renderer = new Renderer(_client);
            _window.AddRenderer(() =>
            {
                var width = Math.Min(_window.ClientSize.X, _window.ClientSize.Y);
                GL.Viewport(0, 0, width, width);
            });
            _window.AddObject(_renderer);
            _window.KeyDown += (_,obj) => _renderer.OnKeyDown(obj);
            _window.KeyUp += (_,obj) => _renderer.OnKeyUp(obj);
        }

        public void Run()
        {
            using var _tickProvider = new Timer(50);
            _tickProvider.Elapsed += (_, _) => _renderer.Tick();
            _tickProvider.Start();
            _window.ReloadWindow();
            _tickProvider.Stop();
        }

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

            public OpenTK.Mathematics.Color4 Color
            {
                get => (OpenTK.Mathematics.Color4)GetVector4(_colorLocation);
                set => SetVector4(_colorLocation, (OpenTK.Mathematics.Vector4)value);
            }
        }

        private class Renderer : ICompletedRenderer
        {
            private Color4 _color = Color.Red;
            private Shader _shader;
            private IVertexArrayHandle _circle;
            private IVertexArrayHandle _arrow1;
            private IVertexArrayHandle _arrow2;
            private Vector3d _rawDelta;
            private Vector3d _delta;
            private readonly MinecraftClient _client;

            public Renderer(MinecraftClient client)
            {
                _client = client;

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
                _arrow1 = new VertexArray<float>(new[] { 0F, 0F, 1F, 0F }, Pointers()).GetHandle();
                _arrow2 = new VertexArray<float>(new[] { -1F, 0F, 1F, 0F }, Pointers()).GetHandle();
            }

            void IRenderable.Render()
            {
                _shader.Use(); 
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                _shader.Color = _color;
                _circle.Bind();
                _circle.Render(PrimitiveType.LineLoop);
                _arrow1.Bind();
                _arrow1.VertexSubData(sizeof(float) * 2, sizeof(float) * 2, new[] { (float)_delta.X, -(float)_delta.Z });
                _arrow1.Render(PrimitiveType.Lines);
                if (Math.Abs(_delta.Y) < 0.01D)
                    return;
                _arrow2.Bind();
                _arrow2.VertexSubData(0, sizeof(float) * 4, new[] { 0, (float)_delta.Y, 1, (float)_delta.Y });
                _arrow2.Render(PrimitiveType.Lines);
            }

            void IUpdatable.Update()
            {
                const float speed = 0.05F;
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
                _delta += d * speed;
            }

            public void OnKeyDown(KeyboardKeyEventArgs e)
            {
                switch (e.Key)
                {
                    case Keys.Up:
                        _rawDelta.Z = -1D;
                        break;
                    case Keys.Down:
                        _rawDelta.Z = 1D;
                        break;
                    case Keys.PageUp:
                        _rawDelta.Y = 1D;
                        break;
                    case Keys.PageDown:
                        _rawDelta.Y = -1D;
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
                        if (Math.Abs(_rawDelta.Z + 1D) < 0.1D)
                            _rawDelta.Z = 0D;
                        break;
                    case Keys.Down:
                        if (Math.Abs(_rawDelta.Z - 1D) < 0.1D)
                            _rawDelta.Z = 0D;
                        break;
                    case Keys.PageUp:
                        if (Math.Abs(_rawDelta.Y - 1D) < 0.1D)
                            _rawDelta.Y = 0D;
                        break;
                    case Keys.PageDown:
                        if (Math.Abs(_rawDelta.Y + 1D) < 0.1D)
                            _rawDelta.Y = 0D;
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

            public void Tick()
            {
                var positionHandler = _client.GetPlayer()?.GetPositionHandler();
                if (positionHandler == null)
                {
                    _color = Color4.Red;
                    return;
                }
                _color = _rawDelta.LengthSquared < 0.001D ? Color4.White : Color4.Aqua;
                (var x, var y, var z) = positionHandler.Position;
                var pos = _delta + (x, y, z);
                positionHandler.SetPosition((pos.X, pos.Y, pos.Z), true);
            }

            void IDisposable.Dispose()
            {

            }
        }
    }
}
