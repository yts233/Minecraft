using Minecraft.Data;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Transforming;
using OpenTK.Mathematics;
using System;

namespace Minecraft.Graphics.Blocking
{
    public class ChunkRenderer : ICompletedRenderer
    {
        private readonly IChunk _chunk;
        private readonly Func<TextureAtlas> _atlasProvider;
        private readonly IMatrixProvider<Matrix4, Vector4> _viewMatrix;
        private readonly IMatrixProvider<Matrix4, Vector4> _projectionMatrix;
        private TextureAtlas _atlas;
        private BlockShader _shader;
        private bool _needUpdate = true;
        private IElementArrayHandle _vertex;

        public ChunkRenderer(IChunk chunk, Func<TextureAtlas> atlasProvider, IMatrixProvider<Matrix4, Vector4> viewMatrix, IMatrixProvider<Matrix4, Vector4> projectionMatrix)
        {
            _chunk = chunk;
            _atlasProvider = atlasProvider;
            _viewMatrix = viewMatrix;
            _projectionMatrix = projectionMatrix;
        }

        public void Dispose()
        {
            (_shader as IDisposable)?.Dispose();
            _shader = null;
        }

        public void Initialize()
        {
            _shader = new BlockShader();
            _shader.Model = Matrix4.Identity;
            _atlas = _atlasProvider();
        }

        public void Render()
        {
            _shader.Use();
            if (_needUpdate)
            {
                _vertex?.DisposeAll();
                var calculator = new ChunkVertexArrayProvider(_chunk, _atlas);
                calculator.Calculate();
                _vertex = calculator.ToElementArray().GetHandle();
                _needUpdate = false;
            }
            ((ITexture)_atlas).Bind();
            _shader.Use();
            _shader.Projection = _projectionMatrix.GetMatrix();
            _shader.View = _viewMatrix.GetMatrix();
            _vertex.Bind();
            _vertex.Render();
        }

        public void Tick()
        {

        }

        public void Update()
        {
            _needUpdate = true;
        }
    }
}