﻿using Minecraft.Data;
using Minecraft.Data.Common.Blocking;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Texturing;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Minecraft.Graphics.Blocking
{
    internal class ChunkVertexArrayProvider : IVertexArrayProvider<BlockVertex>, ICalculator
    {
        private static readonly float[] Vertices = new[] {
            //      顶点坐标        纹理坐标            法线
            // front
            1.0f, 0.0f, 0.0f,   0.0f, 0.0f,     0.0f,  0.0f,  -1.0f,
            0.0f, 0.0f, 0.0f,   1.0f, 0.0f,     0.0f,  0.0f,  -1.0f,
            0.0f, 1.0f, 0.0f,   1.0f, 1.0f,     0.0f,  0.0f,  -1.0f,
            1.0f, 1.0f, 0.0f,   0.0f, 1.0f,     0.0f,  0.0f,  -1.0f,
            // back
            0.0f, 0.0f, 1.0f,   0.0f, 0.0f,     0.0f,  0.0f,  1.0f,
            1.0f, 0.0f, 1.0f,   1.0f, 0.0f,     0.0f,  0.0f,  1.0f,
            1.0f, 1.0f, 1.0f,   1.0f, 1.0f,     0.0f,  0.0f,  1.0f,
            0.0f, 1.0f, 1.0f,   0.0f, 1.0f,     0.0f,  0.0f,  1.0f,
            // left
            0.0f, 1.0f, 1.0f,   1.0f, 1.0f,     -1.0f,  0.0f,  0.0f,
            0.0f, 1.0f, 0.0f,   0.0f, 1.0f,     -1.0f,  0.0f,  0.0f,
            0.0f, 0.0f, 0.0f,   0.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
            0.0f, 0.0f, 1.0f,   1.0f, 0.0f,     -1.0f,  0.0f,  0.0f,
            // right
            1.0f, 0.0f, 0.0f,   1.0f, 0.0f,     1.0f,  0.0f,  0.0f,
            1.0f, 1.0f, 0.0f,   1.0f, 1.0f,     1.0f,  0.0f,  0.0f,
            1.0f, 1.0f, 1.0f,   0.0f, 1.0f,     1.0f,  0.0f,  0.0f,
            1.0f, 0.0f, 1.0f,   0.0f, 0.0f,     1.0f,  0.0f,  0.0f,
            // bottom
            0.0f, 0.0f, 0.0f,   0.0f, 1.0f,     0.0f,  -1.0f,  0.0f,
            1.0f, 0.0f, 0.0f,   1.0f, 1.0f,     0.0f,  -1.0f,  0.0f,
            1.0f, 0.0f, 1.0f,   1.0f, 0.0f,     0.0f,  -1.0f,  0.0f,
            0.0f, 0.0f, 1.0f,   0.0f, 0.0f,     0.0f,  -1.0f,  0.0f,
            // top
            0.0f, 1.0f, 1.0f,   0.0f, 0.0f,     0.0f,  1.0f,  0.0f,
            1.0f, 1.0f, 1.0f,   1.0f, 0.0f,     0.0f,  1.0f,  0.0f,
            1.0f, 1.0f, 0.0f,   1.0f, 1.0f,     0.0f,  1.0f,  0.0f,
            0.0f, 1.0f, 0.0f,   0.0f, 1.0f,     0.0f,  1.0f,  0.0f,
        };

        private static readonly uint[] Indices = new uint[] {
            0,1,2,
            2,3,0,
            //
            //4,5,6,
            //6,7,4,
            //
            //8,9,10,
            //10,11,8,
            //
            //12,13,14,
            //14,15,12,
            //
            //16,17,18,
            //18,19,16,
            //
            //20,21,22,
            //22,23,20
        };

        private uint[] _indices;
        private BlockVertex[] _vertices;
        private readonly IChunk _chunk;
        private readonly Func<ITextureAtlas> _atlasProvider;

        public ChunkVertexArrayProvider(IChunk chunk, Func<ITextureAtlas> atlasProvider)
        {
            _chunk = chunk;
            _atlasProvider = atlasProvider;
        }

        private void AddVertices(int face, List<uint> indices, List<BlockVertex> vertices, int bx, int by, int bz, ref Box2 uv, ref uint count)
        {
            var offset = face * 8 * 4;
            //var index = face * 6;
            for (int i = 0; i < 6; i++)
                indices.Add(Indices[i] + count);
            count += 4;
            for (int i = 0; i < 4; i++)
            {
                var vertex = new BlockVertex
                {
                    X = Vertices[offset++] + bx,
                    Y = Vertices[offset++] + by,
                    Z = Vertices[offset++] + bz,
                    U = Vertices[offset++] == 0.0F ? uv.Min.X : uv.Max.X,
                    V = Vertices[offset++] == 0.0F ? uv.Min.Y : uv.Max.Y,
                    NX = Vertices[offset++],
                    NY = Vertices[offset++],
                    NZ = Vertices[offset++]
                };
                vertices.Add(vertex);
            }
        }

        public void Calculate()
        {
            var texture = _atlasProvider();
            var vertices = new List<BlockVertex>();
            var indices = new List<uint>();
            var count = 0U;
            for (var y = 0; y < 256; y++)
            {
                for (var z = 0; z < 16; z++)
                {
                    for (var x = 0; x < 16; x++)
                    {
                        var block = _chunk.GetBlock(x, y, z);
                        if (block.IsAir())
                            continue;
                        var bx = (_chunk.X << 0x04) | x;
                        var by = y;
                        var bz = (_chunk.Z << 0x04) | z;

                        var uv = texture[new NamedIdentifier(block.Name.Namespace, "block/" + block.Name.Name + ".png")];

                        if (!_chunk.IsTile(x, y, z - 1))
                            AddVertices(0, indices, vertices, bx, by, bz, ref uv, ref count);
                        if (!_chunk.IsTile(x, y, z + 1))
                            AddVertices(1, indices, vertices, bx, by, bz, ref uv, ref count);
                        if (!_chunk.IsTile(x - 1, y, z))
                            AddVertices(2, indices, vertices, bx, by, bz, ref uv, ref count);
                        if (!_chunk.IsTile(x + 1, y, z))
                            AddVertices(3, indices, vertices, bx, by, bz, ref uv, ref count);
                        if (!_chunk.IsTile(x, y - 1, z))
                            AddVertices(4, indices, vertices, bx, by, bz, ref uv, ref count);
                        if (!_chunk.IsTile(x, y + 1, z))
                            AddVertices(5, indices, vertices, bx, by, bz, ref uv, ref count);
                    }
                }
            }
            _vertices = vertices.ToArray();
            _indices = indices.ToArray();
            Logger.GetLogger<ChunkVertexArrayProvider>().Info($"Chunk {_chunk.X}, {_chunk.Z} Calculated.");
        }

        public IEnumerable<uint> GetIndices()
        {
            return _indices;
        }

        public IEnumerable<VertexAttributePointer> GetPointers()
        {
            yield return new VertexAttributePointer
            {
                Index = 0,
                Normalized = false,
                Offset = 0,
                Size = 3,
                Type = VertexAttribePointerType.Float
            };
            yield return new VertexAttributePointer
            {
                Index = 1,
                Normalized = false,
                Offset = 3 * sizeof(float),
                Size = 2,
                Type = VertexAttribePointerType.Float
            };
            yield return new VertexAttributePointer
            {
                Index = 2,
                Normalized = false,
                Offset = 5 * sizeof(float),
                Size = 3,
                Type = VertexAttribePointerType.Float
            };
        }

        public IEnumerable<BlockVertex> GetVertices()
        {
            return _vertices;
        }
    }
}