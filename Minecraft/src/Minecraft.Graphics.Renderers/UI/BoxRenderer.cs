using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Transforming;
using OpenTK.Mathematics;
using System;

namespace Minecraft.Graphics.Renderers.UI
{
    public class BoxRenderer : IInitializer, IRenderable
    {
        private BoxShader _shader;
        private IElementArrayHandle _eah;

        public Color4 Color { get; set; }
        public Box3 Box { get; set; }

        private readonly IMatrixProvider<Matrix4, Vector4> _projectionMatrix;
        private readonly IMatrixProvider<Matrix4, Vector4> _viewMatrix;

        public BoxRenderer(IMatrixProvider<Matrix4, Vector4> viewMatrix, IMatrixProvider<Matrix4, Vector4> projectionMatrix)
        {
            _projectionMatrix = projectionMatrix;
            _viewMatrix = viewMatrix;
        }

        public void Dispose()
        {
            (_shader as IDisposable)?.Dispose();
            _eah?.DisposeAll();
        }

        public void Initialize()
        {
            _shader = new BoxShader();
            _eah = new VertexArray<float>(new float[]
            {
                0F,0F,0F,
                1F,0F,0F,
                1F,0F,1F,
                0F,0F,1F,
                0F,1F,0F,
                1F,1F,0F,
                1F,1F,1F,
                0F,1F,1F,
            }, new[]
            {
                new VertexAttributePointer()
                {
                    Index=0,
                    Normalized=false,
                    Offset=0,
                    Size=3,
                    Type=VertexAttribePointerType.Float
                }
            }).ToElementArray(new uint[]
            {
                0,1,
                1,2,
                2,3,
                3,0,
                4,5,
                5,6,
                6,7,
                7,4,
                0,4,
                1,5,
                2,6,
                3,7
            }).GetHandle();
        }

        public void Render()
        {
            _shader.Use();
            _shader.Color = Color;
            _shader.Model = Matrix4.CreateTranslation(Box.Min) * Matrix4.CreateScale(Box.Size);
            _shader.View = _viewMatrix.GetMatrix();
            _shader.Projection = _projectionMatrix.GetMatrix();

            _eah.Bind();
            _eah.Render(OpenTK.Graphics.OpenGL4.PrimitiveType.Lines);
        }
    }
}
