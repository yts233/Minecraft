using System;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Transforming;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.Debuggers.Axis
{
    public class AxisRenderer : ICompletedRenderer
    {
        private readonly IMatrixProvider _viewMatrix;
        private readonly IMatrixProvider _projectionMatrix;
        private AxisShader _shader;
        private IVertexArrayHandle _vertexArray;

        public AxisRenderer(IMatrixProvider viewMatrix,IMatrixProvider projectionMatrix)
        {
            _viewMatrix = viewMatrix;
            _projectionMatrix = projectionMatrix;
        }

        public void Initialize()
        {
            _shader = new AxisShader();
            _vertexArray = new AxisVertexProvider().ToVertexArray().GetHandle();
        }

        public void Render()
        {
            var matrix = _viewMatrix.GetMatrix();
            matrix.Column3 = Vector4.UnitW;
            _shader.View = matrix;
            _shader.Projection = Matrix4.Identity;
            _vertexArray.Render(PrimitiveType.Lines);
        }

        public void Update()
        {
        }

        public void Bind()
        {
            _shader.Use();
            _vertexArray.Bind();
        }

        public void Tick()
        {
        }

        public void Dispose()
        {
            (_shader as IDisposable)?.Dispose();
            _shader = null;
            _vertexArray?.Dispose();
            _vertexArray = null;
        }
    }
}