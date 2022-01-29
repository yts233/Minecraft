using Minecraft.Data;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Transforming;
using OpenTK.Mathematics;
using System;

namespace Minecraft.Graphics.Renderers.Blocking
{
    /// <summary>
    /// 提供一个异步的区块渲染器
    /// </summary>
    /// <remarks>see <see cref="ChunkCalculatorThread"/></remarks>
    public class ChunkRenderer : ICompletedRenderer
    {
        public static IThreadDispatcher ChunkCalculatorThread { get; } = ThreadHelper.CreateDispatcher("ChunkCalculatorThread");
        private readonly IChunk _chunk;
        private readonly Func<ITexture2DAtlas> _textureAtlasProvider;
        private readonly IMatrixProvider<Matrix4, Vector4> _viewMatrix;
        private readonly IMatrixProvider<Matrix4, Vector4> _projectionMatrix;
        private ChunkVertexArrayProvider _chunkVertexArrayProvider;
        private ITexture2DAtlas _textureDictionary;
        private BlockShader _shader;
        private bool _needUpdate = false;
        private IElementArrayHandle _vertex;

        public ChunkRenderer(IChunk chunk, Func<ITexture2DAtlas> textureAtlasProvider, IMatrixProvider<Matrix4, Vector4> viewMatrix, IMatrixProvider<Matrix4, Vector4> projectionMatrix)
        {
            _chunk = chunk;
            _textureAtlasProvider = textureAtlasProvider;
            _viewMatrix = viewMatrix;
            _projectionMatrix = projectionMatrix;
        }

        public void Dispose()
        {
            (_shader as IDisposable)?.Dispose();
            _shader = null;
        }

        internal void InitializeWithShader(BlockShader shader)
        {
            _shader = shader;
            _shader.Model = Matrix4.Identity;
            _textureDictionary = _textureAtlasProvider();
            _chunkVertexArrayProvider = new ChunkVertexArrayProvider(_chunk, _textureAtlasProvider);
            Logger.GetLogger<ChunkRenderer>().Info($"Initalized {_chunk.X}, {_chunk.Z}");
        }

        public void Initialize()
        {
            _shader = new BlockShader();
            _shader.Model = Matrix4.Identity;
            _textureDictionary = _textureAtlasProvider();
            _chunkVertexArrayProvider = new ChunkVertexArrayProvider(_chunk, _textureAtlasProvider);
            Logger.GetLogger<ChunkRenderer>().Info($"Initalized {_chunk.X}, {_chunk.Z}");
            GenerateMeshes();
        }

        public void Render()
        {
            if (_needUpdate)
            {
                if (_chunkVertexArrayProvider == null)
                {
                    _chunkVertexArrayProvider = new ChunkVertexArrayProvider(_chunk, _textureAtlasProvider);
                    _chunkVertexArrayProvider.Calculate();
                }
                else
                {
                    _vertex?.DisposeAll();
                    _vertex = _chunkVertexArrayProvider.ToElementArray().GetHandle();
                    _needUpdate = false;
                }
            }
            if (_vertex == null)
                return;
            _textureDictionary.Bind();
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
            ChunkCalculatorThread.Invoke(() =>
            {
                _chunkVertexArrayProvider?.Calculate();
                _needUpdate = true;
            }, true);
        }
    }
}