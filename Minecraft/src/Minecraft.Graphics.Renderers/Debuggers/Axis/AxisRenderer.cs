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
        private readonly IMatrixProvider<Matrix4,Vector4> _viewMatrix;
        private readonly IMatrixProvider<Matrix4,Vector4> _projectionMatrix;
        private AxisShader _shader;
        private IVertexArrayHandle _vertexArray;

        public AxisRenderer(IMatrixProvider<Matrix4,Vector4> viewMatrix,IMatrixProvider<Matrix4,Vector4> projectionMatrix)
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
            _shader.Use();
            _shader.View = matrix;
            _shader.Projection = _projectionMatrix.GetMatrix();
            _vertexArray.Bind();
            _vertexArray.Render(PrimitiveType.Lines);
        }

        public void Update()
        {
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