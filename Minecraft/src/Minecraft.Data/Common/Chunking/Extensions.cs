using System.Collections.Generic;
using System.Linq;
using Minecraft.Data.Common.Blocking;

namespace Minecraft.Data.Common.Chunking
{
    public static class Extensions
    {
        /*
         * position in chunk section:
         * |    Y    |    Z    |    X    |
         * | 0 1 2 3 | 4 5 6 7 | 8 9 A B |
         * 
         * position in chunk section:
         * |        Y        |    Z    |    X    |
         * | 0 1 2 3 4 5 6 7 | 8 9 A B | C D E F |
         * 
         */

        public static int ToBlockPositionInChunk(this (int x, int y, int z) position)
        {
            return (position.y << 8) | (position.z << 4) | position.x;
        }

        public static (int x, int y, int z) ToBlockPositionInChunk(this int position)
        {
            return (position & 0x0F, (position >> 8) & 0xFF, (position >> 4) & 0x0F);
        }

        private static int GetBlockPosition(int x, int y, int z)
        {
            return (y << 8) | (z << 4) | x;
        }

        private static (int chunkX, int chunkY, int chunkZ) GetChunkPosition(int blockX, int blockY, int blockZ)
        {
            return (blockX >> 4, blockY >> 4, blockZ >> 4);
        }

        private static (int blockInChunkX, int blockInChunkY, int blockInChunkZ) GetBlockInChunkPosition(int blockX,
            int blockY, int blockZ)
        {
            return (blockX & 0x0F, blockY & 0x0F, blockZ & 0x0F);
        }

        private static sbyte Nibble4(sbyte[] arr, int index)
        {
            return (sbyte) ((index & 0x01) == 0 ? arr[index / 2] & 0x0F : arr[index / 2] >> 4);
        }

        private static void Nibble4(sbyte[] arr, int index, sbyte value)
        {
            arr[index / 2] |= (index & 0x01) == 0
                ? (sbyte) (arr[index / 2] & 0x0F)
                : (sbyte) (arr[index / 2] >> 4);
        }

        // TODO: handle the relation among locations

        public static ChunkSection GetSectionOrNull(this ChunkData chunkData, sbyte sectionY)
        {
            return chunkData.Sections
                .FirstOrDefault(section => section.Y == sectionY);
        }

        public static ChunkSection GetSectionOrCreate(this ChunkData chunkData, sbyte sectionY)
        {
            var section = chunkData.GetSectionOrNull(sectionY);
            if (section != null)
                return section;
            section = new ChunkSection {Y = sectionY};
            chunkData.Sections.Add(section);
            return section;
        }

        public static int? GetPaletteIndexOrNull(this ChunkSection chunkSection, BlockState blockState)
        {
            if (!chunkSection.Palette.Contains(blockState))
                return null;
            return chunkSection.Palette.IndexOf(blockState);
        }

        public static int GetPaletteIndexOrCreate(this ChunkSection chunkSection, BlockState blockState)
        {
            if (!chunkSection.Palette.Contains(blockState))
                chunkSection.Palette.Add(blockState);
            return chunkSection.Palette.IndexOf(blockState);
        }

        public static IEnumerable<(int blockPosition, BlockState blockState)> GetBlocks(this ChunkSection chunkSection)
        {
            for (var i = 0; i < 4096; i++)
                yield return (i, chunkSection.GetBlock(i));
        }

        public static IEnumerable<(int blockPosition, BlockState blockState)> GetBlocks(this ChunkData chunkData)
        {
            for (sbyte sectionY = 0; sectionY < 16; sectionY++)
            {
                var section = chunkData.GetSectionOrNull(sectionY);
                var tmp1 = sectionY << 12;
                if (section == null)
                {
                    var tmp2 = tmp1 + 4096;
                    for (var i = tmp1; i < tmp2; i++)
                        yield return (i, new BlockState("void_air"));
                    continue;
                }

                for (var i = 0; i < 4096; i++)
                    yield return (tmp1 + i, section.GetBlock(i));
            }
        }

        public static BlockState GetBlock(this ChunkSection chunkSection, int blockPosition)
        {
            return chunkSection.Palette[(int) chunkSection.BlockStates[blockPosition]];
        }

        public static BlockState GetBlock(this ChunkSection chunkSection, int x, int y, int z)
        {
            return chunkSection.GetBlock(GetBlockPosition(x, y, z));
        }

        public static BlockState GetBlock(this ChunkData chunkData, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            return chunkData.GetSectionOrNull(sectionY)?
                       .GetBlock(x, inSectionY, z)
                   ?? new BlockState("void_air");
        }

        public static void SetBlock(this ChunkSection chunkSection, BlockState blockState, int blockPosition)
        {
            var index = chunkSection.GetPaletteIndexOrCreate(blockState);
            chunkSection.BlockStates[blockPosition] = index;
        }

        public static void SetBlock(this ChunkSection chunkSection, BlockState blockState, int x, int y, int z)
        {
            chunkSection.SetBlock(blockState, GetBlockPosition(x, y, z));
        }

        public static void SetBlock(this ChunkData chunkData, BlockState blockState, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            var section = chunkData.GetSectionOrCreate(sectionY);
            section.SetBlock(blockState, x, inSectionY, z);
        }

        public static sbyte GetBlockLight(this ChunkSection chunkSection, int blockPosition)
        {
            return Nibble4(chunkSection.BlockLight, blockPosition);
        }

        public static sbyte GetBlockLight(this ChunkSection chunkSection, int x, int y, int z)
        {
            return chunkSection.GetBlockLight(GetBlockPosition(x, y, z));
        }

        public static sbyte GetBlockLight(this ChunkData chunkData, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            return chunkData.GetSectionOrNull(sectionY)?
                       .GetBlockLight(x, inSectionY, z)
                   ?? 0;
        }

        public static void SetBlockLight(this ChunkSection chunkSection, sbyte light, int blockPosition)
        {
            Nibble4(chunkSection.BlockLight, blockPosition, light);
        }

        public static void SetBlockLight(this ChunkSection chunkSection, sbyte light, int x, int y, int z)
        {
            chunkSection.SetBlockLight(light, GetBlockPosition(x, y, z));
        }

        public static void SetBlockLight(this ChunkData chunkData, sbyte light, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            var section = chunkData.GetSectionOrCreate(sectionY);
            section.SetBlockLight(light, x, inSectionY, z);
        }

        public static sbyte GetSkyLight(this ChunkSection chunkSection, int blockPosition)
        {
            return Nibble4(chunkSection.SkyLight, blockPosition);
        }

        public static sbyte GetSkyLight(this ChunkSection chunkSection, int x, int y, int z)
        {
            return chunkSection.GetSkyLight(GetBlockPosition(x, y, z));
        }

        public static sbyte GetSkyLight(this ChunkData chunkData, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            return chunkData.GetSectionOrNull(sectionY)?
                       .GetSkyLight(x, inSectionY, z)
                   ?? 0;
        }

        public static void SetSkyLight(this ChunkSection chunkSection, sbyte light, int blockPosition)
        {
            Nibble4(chunkSection.SkyLight, blockPosition, light);
        }

        public static void SetSkyLight(this ChunkSection chunkSection, sbyte light, int x, int y, int z)
        {
            chunkSection.SetBlockLight(light, GetBlockPosition(x, y, z));
        }

        public static void SetSkyLight(this ChunkData chunkData, sbyte light, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            var section = chunkData.GetSectionOrCreate(sectionY);
            section.SetSkyLight(light, x, inSectionY, z);
        }
    }
}