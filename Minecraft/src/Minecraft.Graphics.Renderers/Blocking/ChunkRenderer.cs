#define SyncCalculateChunk

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
    public class ChunkRenderer : ICompletedRenderer
    {
#if SyncCalculateChunk
        private static IThreadDispatcher ChunkCalculatorThread { get; } = ThreadHelper.CreateDispatcher("ChunkCalculatorThread");
#endif
        private readonly IChunk _chunk;
        private readonly Func<ITexture2DAtlas> _textureAtlasProvider;
        private readonly IMatrixProvider<Matrix4, Vector4> _viewMatrix;
        private readonly IMatrixProvider<Matrix4, Vector4> _projectionMatrix;
        private ChunkVertexArrayProvider _chunkVertexArrayProvider;
        private ITexture2DAtlas _textureDictionary;
        private BlockShader _shader;
        private bool _shaderOwner = true;
        private bool _needUpdate = false;
        private IElementArrayHandle _vertex;

        public ChunkRenderer(IChunk chunk, Func<ITexture2DAtlas> textureAtlasProvider, IMatrixProvider<Matrix4, Vector4> viewMatrix, IMatrixProvider<Matrix4, Vector4> projectionMatrix)
        {
            _chunk = chunk;
            _textureAtlasProvider = textureAtlasProvider;
            _viewMatrix = viewMatrix;
            _projectionMatrix = projectionMatrix;
        }

        internal bool Disposed { get; private set; }

        public void Dispose()
        {
            if (Disposed)
                return;
            lock (_renderLock)
            {
                Disposed = true;
                if (_shaderOwner)
                    (_shader as IDisposable)?.Dispose();
                _shader = null;
                _chunkVertexArrayProvider.StopCalculation();
                _chunkVertexArrayProvider = null;
            }
        }

        internal void InitializeWithShader(BlockShader shader)
        {
            Disposed = false;
            _shaderOwner = false;
            _shader = shader;
            /*_shader.Model = Matrix4.Identity;*/
            _textureDictionary = _textureAtlasProvider();
            _chunkVertexArrayProvider = new ChunkVertexArrayProvider(_chunk, _textureAtlasProvider);
            //Logger.GetLogger<ChunkRenderer>().Info($"Initalized {_chunk.X}, {_chunk.Z}");
        }

        public void Initialize()
        {
            Disposed = false;
            _shader = new BlockShader();
            _shader.Model = Matrix4.Identity;
            _textureDictionary = _textureAtlasProvider();
            _chunkVertexArrayProvider = new ChunkVertexArrayProvider(_chunk, _textureAtlasProvider);
            Logger.GetLogger<ChunkRenderer>().Info($"Initalized {_chunk.X}, {_chunk.Z}");
            GenerateMeshes();
        }

        private readonly object _updateLock = new object();
        private readonly object _renderLock = new object();

        public void Render()
        {
            if (Disposed)
                return;
            if (_needUpdate)
            {
                lock (_updateLock)
                {
                    _vertex?.DisposeAll();
                    _vertex = _chunkVertexArrayProvider.ToElementArray().GetHandle();
                    _chunkVertexArrayProvider = null;
                    _needUpdate = false;
                }
            }
            lock (_renderLock)
            {
                if (_vertex == null || _shader == null)
                    return;
                _textureDictionary.Bind();
                _shader.Use();
                _shader.Model = Matrix4.Identity;
                _shader.Projection = _projectionMatrix.GetMatrix();
                _shader.View = _viewMatrix.GetMatrix();
                _vertex.Bind();
                _vertex.Render();
            }
        }

        public void Tick()
        {

        }

        public void Update()
        {

        }

        public void GenerateMeshes()
        {
            if (Disposed)
                return;
#if SyncCalculateChunk
            ChunkCalculatorThread.Invoke(() =>
#else
            ThreadHelper.StartThread(() =>
#endif
            {
                lock (_updateLock)
                {
                    _chunkVertexArrayProvider = new ChunkVertexArrayProvider(_chunk, _textureAtlasProvider);
                    _needUpdate = _chunkVertexArrayProvider.Calculate();
                }
#if SyncCalculateChunk
            }, true);
#else
            }, "ChunkCalculatorThread", true);
#endif
        }
    }
}