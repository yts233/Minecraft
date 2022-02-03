using Minecraft.Data;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Transforming;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minecraft.Graphics.Renderers.Blocking
{
    public class WorldRenderer : ICompletedRenderer
    {
        private readonly IWorld _world;
        private readonly Func<ITexture2DAtlas> _textureAtlasProvider;
        private readonly IMatrixProvider<Matrix4, Vector4> _viewMatrix;
        private readonly IMatrixProvider<Matrix4, Vector4> _projectionMatrix;
        private readonly Dictionary<(int x, int z), ChunkRenderer> _renderers = new Dictionary<(int x, int z), ChunkRenderer>();
        private readonly Queue<ChunkRenderer> _updates = new Queue<ChunkRenderer>();
        private readonly Queue<ChunkRenderer> _initalizes = new Queue<ChunkRenderer>();
        private ITexture2DAtlas _textureDictionary;
        private BlockShader _shader;

        public int CenterChunkX { get; set; } = 0;
        public int CenterChunkZ { get; set; } = 0;
        public int ViewDistance { get; set; } = 8;
        public int CachedDistance { get; set; } = 16;

        public ICamera Camera { get; set; }
        public bool AutoSetCenterChunk { get; set; }

        public WorldRenderer(IWorld world, Func<ITexture2DAtlas> textDictionaryProvider, IMatrixProvider<Matrix4, Vector4> viewMatrix, IMatrixProvider<Matrix4, Vector4> projectionMatrix)
        {
            _world = world;
            _textureAtlasProvider = textDictionaryProvider;
            _viewMatrix = viewMatrix;
            _projectionMatrix = projectionMatrix;
        }

        public void Dispose()
        {
        }

        public void Initialize()
        {
            _textureDictionary = _textureAtlasProvider();
            foreach (var (_, renderer) in _renderers)
                renderer.Dispose();
            _renderers.Clear();
            _shader = new BlockShader();
        }

        private const int MaxNewChunkCountPerFrame = 1;

        public void Render()
        {
            while (_initalizes.TryDequeue(out var renderer))
            {
                renderer.InitializeWithShader(_shader);
            }

            var minRenderX = CenterChunkX - ViewDistance / 2;
            var minRenderZ = CenterChunkZ - ViewDistance / 2;
            var maxRenderX = CenterChunkX + ViewDistance / 2;
            var maxRenderZ = CenterChunkZ + ViewDistance / 2;

            var newRenderers = new List<ChunkRenderer>();

            int newChunkCount = 0;
            lock (_renderers)
                for (var z = minRenderZ; z <= maxRenderZ; z++)
                {
                    for (var x = minRenderX; x <= maxRenderX; x++)
                    {
                        if (!_renderers.TryGetValue((x, z), out ChunkRenderer renderer))
                        {
                            var chunk = _world.GetChunk(x, z);
                            if (newChunkCount == MaxNewChunkCountPerFrame || chunk == null || chunk.IsEmpty)
                                continue;
                            renderer = new ChunkRenderer(chunk, () => _textureDictionary, _viewMatrix, _projectionMatrix);
                            _renderers.Add((x, z), renderer);
                            renderer.InitializeWithShader(_shader);
                            newRenderers.Add(renderer);
                            newChunkCount++;
                            continue;
                        }
                    }
                }

            List<ChunkRenderer> toRender;
            lock (_renderers)
                toRender = _renderers.Values.ToList();
            foreach (var renderer in toRender)
                if (!renderer.Disposed)
                    renderer.Render();

            if (newRenderers.Count > 0)
                lock (_updates)
                    foreach (var renderer in newRenderers)
                        _updates.Enqueue(renderer);
        }

        public void Tick()
        {
            while (true)
            {
                ChunkRenderer renderer;
                lock (_updates)
                    if (!_updates.TryDequeue(out renderer))
                        break;
                renderer.GenerateMeshes();
            }

            var minCachedX = CenterChunkX - CachedDistance / 2;
            var minCachedZ = CenterChunkZ - CachedDistance / 2;
            var maxCachedX = CenterChunkX + CachedDistance / 2;
            var maxCachedZ = CenterChunkZ + CachedDistance / 2;

            lock (_renderers)
                foreach (var (key, value) in _renderers.Where(kvp => kvp.Key.x < minCachedX || kvp.Key.z < minCachedZ || kvp.Key.x > maxCachedX || kvp.Key.z > maxCachedZ).ToList())
                {
                    _renderers.Remove(key);
                    value.Dispose();
                }
        }

        public void Update()
        {
            if (AutoSetCenterChunk && Camera != null)
            {
                CenterChunkX = (int)Camera.Position.X >> 4;
                CenterChunkZ = (int)Camera.Position.Z >> 4;
            }
        }

        public void MarkUpdate(int x, int z)
        {
            lock (_renderers)
            {
                if (!_renderers.TryGetValue((x, z), out ChunkRenderer renderer))
                {
                    var chunk = _world.GetChunk(x, z);
                    if (chunk == null)
                        return;
                    renderer = new ChunkRenderer(chunk, () => _textureDictionary, _viewMatrix, _projectionMatrix);
                    _renderers.Add((x, z), renderer);
                    lock (_initalizes)
                        _initalizes.Enqueue(renderer);
                }
                lock (_updates)
                    _updates.Enqueue(renderer);
            }
        }

        public void MarkUpdate()
        {
            var minRenderX = CenterChunkX - ViewDistance / 2;
            var minRenderZ = CenterChunkZ - ViewDistance / 2;
            var maxRenderX = CenterChunkZ + ViewDistance / 2;
            var maxRenderZ = CenterChunkZ + ViewDistance / 2;

            lock (_renderers)
            {
                for (var z = minRenderZ; z <= maxRenderZ; z++)
                {
                    for (var x = minRenderX; x <= maxRenderX; x++)
                    {
                        if (!_renderers.TryGetValue((x, z), out ChunkRenderer renderer))
                        {
                            var chunk = _world.GetChunk(x, z);
                            if (chunk == null)
                                continue;
                            renderer = new ChunkRenderer(chunk, () => _textureDictionary, _viewMatrix, _projectionMatrix);
                            _renderers.Add((x, z), renderer);
                            lock (_initalizes)
                                _initalizes.Enqueue(renderer);
                        }
                        lock (_updates)
                            _updates.Enqueue(renderer);
                    }
                }
            }
        }
    }
}