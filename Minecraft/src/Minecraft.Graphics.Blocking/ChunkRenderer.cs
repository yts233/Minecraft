using Minecraft.Data;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Transforming;
using OpenTK.Mathematics;
using System;

namespace Minecraft.Graphics.Blocking
{
    /// <summary>
    /// 提供一个异步队列的区块渲染器
    /// </summary>
    /// <remarks>see <see cref="ChunkRendererThread"/></remarks>
    public class ChunkRenderer : ICompletedRenderer
    {
        public static IThreadDispatcher ChunkRendererThread { get; } = ThreadHelper.NewThread("ChunkRendererThread");
        private readonly IChunk _chunk;
        private readonly Func<ITextureAtlas> _atlasProvider;
        private readonly IMatrixProvider<Matrix4, Vector4> _viewMatrix;
        private readonly IMatrixProvider<Matrix4, Vector4> _projectionMatrix;
        private ChunkVertexArrayProvider _chunkVertexArrayProvider;
        private ITextureAtlas _atlas;
        private BlockShader _shader;
        private bool _needUpdate = false;
        private IElementArrayHandle _vertex;

        public ChunkRenderer(IChunk chunk, Func<ITextureAtlas> atlasProvider, IMatrixProvider<Matrix4, Vector4> viewMatrix, IMatrixProvider<Matrix4, Vector4> projectionMatrix)
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
            _chunkVertexArrayProvider = new ChunkVertexArrayProvider(_chunk, _atlas);
            Logger.GetLogger<ChunkRenderer>().Info($"Initalized {_chunk.X}, {_chunk.Z}");
            GenerateMeshes();
        }

        public void Render()
        {
            _shader.Use();
            if (_needUpdate)
            {
                _vertex?.DisposeAll();
                _vertex = _chunkVertexArrayProvider.ToElementArray().GetHandle();
                _needUpdate = false;
            }
            if (_vertex == null)
                return;
            _atlas.Bind();
            _shader.Use();
            _shader.Model = Matrix4.Identity;
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

        }

        public void GenerateMeshes()
        {
            ChunkRendererThread.Invoke(() =>
            {
                _chunkVertexArrayProvider.Calculate();
                _needUpdate = true;
            }, true);
        }
    }
}