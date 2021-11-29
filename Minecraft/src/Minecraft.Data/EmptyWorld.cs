﻿using Minecraft.Data.Common.Blocking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Data
{
    public class EmptyWorld : IEditableWorld
    {
        private readonly List<IChunk> _chunks = new List<IChunk>();

        public bool AddChunk(IChunk chunk)
        {
            var x = chunk.X;
            var z = chunk.Z;
            if (HasChunk(x, z))
                return false;
            _chunks.Add(chunk);
            return true;
        }

        public IEnumerable<(int x, int y, int z, BlockState block)> EnumerateBlocks()
        {
            foreach (var chunk in _chunks)
            {
                int cx = chunk.X << 4;
                int cz = chunk.Z << 4;
                foreach ((var x, var y, var z, var block) in chunk.EnumerateBlocks())
                    yield return (cx + x, y, cz + z, block);
            }
        }

        public IEnumerable<IChunk> EnumerateChunks()
        {
            return _chunks;
        }

        public BlockState GetBlock(int x, int y, int z)
        {
            if (y < 0x00 || y > 0xff)
                return "void_air";
            return GetChunk(x >> 4, z >> 4)?.GetBlock(x & 0x03, y, z & 0x03) ?? "void_air";
        }

        public IChunk GetChunk(int x, int z)
        {
            foreach (var chunk in _chunks)
                if (chunk.X == x && chunk.Z == z)
                    return chunk;
            return null;
        }

        public bool HasChunk(int x, int z)
        {
            return GetChunk(x, z) != null;
        }

        public bool IsTile(int x, int y, int z)
        {
            if (y < 0x00 || y > 0xff)
                return false;
            return GetChunk(x >> 4, z >> 4)?.IsTile(x & 0x03, y, z & 0x03) ?? false;
        }

        public bool RemoveChunk(int x, int z)
        {
            return _chunks.Remove(GetChunk(x, z));
        }

        public bool SetBlock(int x, int y, int z, BlockState block)
        {
            if (y < 0x00 || y > 0xff)
                return false;
            return (GetChunk(x >> 4, z >> 4) is IBlockEditor editor) && editor.SetBlock(x & 0x0F, y, z & 0x0F, block);
        }
    }
}