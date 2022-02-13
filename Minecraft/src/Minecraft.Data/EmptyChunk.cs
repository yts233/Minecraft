using Minecraft.Data.Common.Blocking;
using System.Collections.Generic;

namespace Minecraft.Data
{
    public class EmptyChunk : IEditableChunk
    {
        public int X { get; set; }

        public int Z { get; set; }

        public bool IsEmpty => BlockCount == 0;

        public int BlockCount { get; private set; }

        public IWorld World { get; set; }

        private readonly BlockState[] _blocks = new BlockState[65536];

        public IEnumerable<(int x, int y, int z, BlockState block)> EnumerateBlocks()
        {
            for (var i = 0; i < 65536; i++)
            {
                var block = _blocks[i];
                if (block.IsAir())
                    continue;
                yield return (i & 0x03, i >> 0x08, (i >> 0x04) & 0x03, block);
            }
        }

        public BlockState GetBlock(int x, int y, int z)
        {
            if (y < 0x00 || y > 0xff)
                return "void_air";
            if (x < 0x00 || x > 0x0f || z < 0x00 || z > 0x0f)
                return World.GetBlock((X << 4) + x, y, (Z << 4) + z);
            return _blocks[x | (y << 8) | (z << 4)] ?? "air";
        }

        public bool IsTile(int x, int y, int z)
        {
            if (y < 0x00 || y > 0xff)
                return false;
            if (x < 0x00 || x > 0x0f || z < 0x00 || z > 0x0f)
                return World?.IsTile((X << 4) + x, y, (Z << 4) + z) ?? false;
            var block = _blocks[x | (y << 8) | (z << 4)];
            return !block.IsAir();
        }

        public bool SetBlock(int x, int y, int z, BlockState block)
        {
            if (y < 0x00 || y > 0xff)
                return false;
            if (x < 0x00 || x > 0x0f || z < 0x00 || z > 0x0f)
                return (World is IBlockEditor editor) && editor.SetBlock((X << 4) + x, y, (Z << 4) + z, block);
            if (block.IsAir())
            {
                if (IsTile(x, y, z))
                    BlockCount--;
            }
            else if (!IsTile(x, y, z))
            {
                BlockCount++;
            }
            _blocks[x | (y << 8) | (z << 4)] = block;
            return true;
        }
    }
}
