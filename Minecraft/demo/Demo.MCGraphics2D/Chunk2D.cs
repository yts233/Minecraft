using Minecraft;
using Minecraft.Data;
using Minecraft.Data.Common.Blocking;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Texturing;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Demo.MCGraphics2D
{
    public class Chunk2D : IBlockEditor, IBlockProvider
    {
        public readonly BlockState[] _blocks = new BlockState[65536];

        public static int Width => 256;
        public static int Height => 256;
        public static int Depth => 1;

        public bool SetBlock(int x, int y, BlockState block)
        {
            if (x < 0 || x > 0xff || y < 0x00 || y > 0xff)
                return false;
            _blocks[(y << 8) | x] = block;
            return true;
        }

        public BlockState GetBlock(int x, int y)
        {
            if (x < 0 || x > 0xff || y < 0x00 || y > 0xff)
                return "void_air";
            return _blocks[(y << 8) | x];
        }

        public bool IsTile(int x, int y)
        {
            if (x < 0 || x > 0xff || y < 0x00 || y > 0xff)
                return false;
            return !_blocks[(y << 8) | x].IsAir();
        }

        private IElementArrayHandle _eah;

        private static readonly Tex2dVertex[] Vertices = new Tex2dVertex[]
        {
            new Tex2dVertex{Position=(0F,0F),TexCoord=(0F,0F)},
            new Tex2dVertex{Position=(1F,0F),TexCoord=(1F,0F)},
            new Tex2dVertex{Position=(1F,1F),TexCoord=(1F,1F)},
            new Tex2dVertex{Position=(0F,1F),TexCoord=(0F,1F)},
        };

        private static readonly uint[] Indices = new uint[]
        {
            0U,1U,2U,
            0U,2U,3U
        };

        public void GenerateMeshes(ITexture2DAtlas texture)
        {
            if (_eah != null)
            {
                _eah.DisposeAll();
                _eah = null;
            }

            var vertices = new List<Tex2dVertex>();
            var indices = new List<uint>();
            var arrow = 0U;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var block = GetBlock(x, y);
                    if (block.IsAir())
                        continue;

                    var box = texture[new NamedIdentifier(block.Name.Namespace, "block/" + block.Name.Name + ".png")];
                    var texCorner = new Vector2[2, 2]
                    {
                        { box.Min, (box.Min.X,box.Max.Y) },
                        { (box.Max.X,box.Min.Y) ,box.Max }
                    };

                    for (int i = 0; i < 4; i++)
                    {
                        var vertex = Vertices[i];
                        vertex.Position += (x, y);
                        vertex.TexCoord = texCorner[(int)vertex.TexCoord.X, (int)vertex.TexCoord.Y];
                        vertices.Add(vertex);
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        indices.Add(Indices[i] + arrow);
                    }

                    arrow += 4U;
                }
            }

            if (vertices.Count == 0)
                return;

            _eah = new VertexArray<Tex2dVertex>(vertices, Tex2dShader.GetPointers()).ToElementArray(indices).GetHandle();
        }

        public IElementArrayHandle GetElementArrayHandle()
        {
            return _eah;
        }

        bool IBlockEditor.SetBlock(int x, int y, int z, BlockState block)
        {
            if (z != 0)
                return false;
            return SetBlock(x, y, block);
        }

        BlockState IBlockProvider.GetBlock(int x, int y, int z)
        {
            if (z == 0)
                return "void_air";
            return GetBlock(x, y);
        }

        IEnumerable<(int x, int y, int z, BlockState block)> IBlockProvider.EnumerateBlocks()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return (x, y, 1, GetBlock(x, y));
                }
            }
        }

        bool IBlockProvider.IsTile(int x, int y, int z)
        {
            if (z != 0)
                return false;
            return IsTile(x, y);
        }

        public IEnumerable<Box2d> GetCubes(Box2d box)
        {
            int x1 = (int)Math.Floor(box.Min.X),
                x2 = (int)Math.Floor(box.Max.X),
                y1 = (int)Math.Floor(box.Min.Y),
                y2 = (int)Math.Floor(box.Max.Y);
            for (int y = y1 - 1; y <= y2 + 1; y++)
                for (int x = x1 - 1; x <= x2 + 1; x++)
                    if (IsTile(x, y))
                        yield return new Box2d((x, y), (x, y) + Vector2d.One);
        }
    }
}
