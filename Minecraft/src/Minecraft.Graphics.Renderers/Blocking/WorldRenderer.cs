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

        public int CenterChunkX { get; set; }
        public int CenterChunkZ { get; set; }
        public int ViewDistance { get; set; }
        public int CachedDistance { get; set; }

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

        public void Render()
        {
            while (_initalizes.TryDequeue(out var renderer))
            {
                renderer.InitializeWithShader(_shader);
            }

            var minRenderX = CenterChunkX - ViewDistance / 2;
            var minRenderZ = CenterChunkZ - ViewDistance / 2;
            var maxRenderX = CenterChunkZ + ViewDistance / 2;
            var maxRenderZ = CenterChunkZ + ViewDistance / 2;
            lock (_renderers)
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
                            renderer.InitializeWithShader(_shader);
                            renderer.GenerateMeshes();
                        }
                        renderer.Render();
                    }
                }

        }

        public void Tick()
        {

        }

        public void Update()
        {
            while (_updates.TryDequeue(out var renderer))
            {
                renderer.GenerateMeshes();
            }

            var minCachedX = CenterChunkX - CachedDistance / 2;
            var minCachedZ = CenterChunkZ - CachedDistance / 2;
            var maxCachedX = CenterChunkZ + CachedDistance / 2;
            var maxCachedZ = CenterChunkZ + CachedDistance / 2;
            foreach (var key in _renderers.Where(kvp => kvp.Key.x < minCachedX || kvp.Key.z < minCachedZ || kvp.Key.x > maxCachedX || kvp.Key.z > maxCachedZ).Select(kvp => kvp.Key).ToList())
                _renderers.Remove(key);
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