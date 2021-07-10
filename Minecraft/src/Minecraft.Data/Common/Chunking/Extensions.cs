using System.Collections.Generic;
using System.Linq;
using Minecraft.Data.Common.Blocking;

namespace Minecraft.Data.Common.Chunking
{
    public static class Extensions
    {
        /*
         * position in chuck section:
         * |    Y    |    Z    |    X    |
         * | 0 1 2 3 | 4 5 6 7 | 8 9 A B |
         * 
         * position in chuck section:
         * |        Y        |    Z    |    X    |
         * | 0 1 2 3 4 5 6 7 | 8 9 A B | C D E F |
         * 
         */

        public static int ToBlockPositionInChuck(this (int x, int y, int z) position)
        {
            return (position.y << 8) | (position.z << 4) | position.x;
        }

        public static (int x, int y, int z) ToBlockPositionInChuck(this int position)
        {
            return (position & 0x0F, (position >> 8) & 0xFF, (position >> 4) & 0x0F);
        }

        private static int GetBlockPosition(int x, int y, int z)
        {
            return (y << 8) | (z << 4) | x;
        }

        private static (int chuckX, int chuckY, int chuckZ) GetChuckPosition(int blockX, int blockY, int blockZ)
        {
            return (blockX >> 4, blockY >> 4, blockZ >> 4);
        }

        private static (int blockInChuckX, int blockInChuckY, int blockInChuckZ) GetBlockInChuckPosition(int blockX,
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

        public static ChuckSection GetSectionOrNull(this ChuckData chuckData, sbyte sectionY)
        {
            return chuckData.Sections
                .Where(section => section.Y == sectionY)
                .FirstOrDefault();
        }

        public static ChuckSection GetSectionOrCreate(this ChuckData chuckData, sbyte sectionY)
        {
            var section = chuckData.GetSectionOrNull(sectionY);
            if (section != null)
                return section;
            section = new ChuckSection {Y = sectionY};
            chuckData.Sections.Add(section);
            return section;
        }

        public static int? GetPaletteIndexOrNull(this ChuckSection chuckSection, BlockState blockState)
        {
            if (!chuckSection.Palette.Contains(blockState))
                return null;
            return chuckSection.Palette.IndexOf(blockState);
        }

        public static int GetPaletteIndexOrCreate(this ChuckSection chuckSection, BlockState blockState)
        {
            if (!chuckSection.Palette.Contains(blockState))
                chuckSection.Palette.Add(blockState);
            return chuckSection.Palette.IndexOf(blockState);
        }

        public static IEnumerable<(int blockPosition, BlockState blockState)> GetBlocks(this ChuckSection chuckSection)
        {
            for (var i = 0; i < 4096; i++)
                yield return (i, chuckSection.GetBlock(i));
        }

        public static IEnumerable<(int blockPosition, BlockState blockState)> GetBlocks(this ChuckData chuckData)
        {
            for (sbyte sectionY = 0; sectionY < 16; sectionY++)
            {
                var section = chuckData.GetSectionOrNull(sectionY);
                var tmp1 = sectionY << 12;
                if (section == null)
                {
                    var tmp2 = tmp1 + 4096;
                    for (var i = tmp1; i < tmp2; i++)
                        yield return (i, new BlockState("minecraft", "air"));
                    continue;
                }

                for (var i = 0; i < 4096; i++)
                    yield return (tmp1 + i, section.GetBlock(i));
            }
        }

        public static BlockState GetBlock(this ChuckSection chuckSection, int blockPosition)
        {
            return chuckSection.Palette[(int) chuckSection.BlockStates[blockPosition]];
        }

        public static BlockState GetBlock(this ChuckSection chuckSection, int x, int y, int z)
        {
            return chuckSection.GetBlock(GetBlockPosition(x, y, z));
        }

        public static BlockState GetBlock(this ChuckData chuckData, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            return chuckData.GetSectionOrNull(sectionY)?
                       .GetBlock(x, inSectionY, z)
                   ?? new BlockState("minecraft", "air");
        }

        public static void SetBlock(this ChuckSection chuckSection, BlockState blockState, int blockPosition)
        {
            var index = chuckSection.GetPaletteIndexOrCreate(blockState);
            chuckSection.BlockStates[blockPosition] = index;
        }

        public static void SetBlock(this ChuckSection chuckSection, BlockState blockState, int x, int y, int z)
        {
            chuckSection.SetBlock(blockState, GetBlockPosition(x, y, z));
        }

        public static void SetBlock(this ChuckData chuckData, BlockState blockState, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            var section = chuckData.GetSectionOrCreate(sectionY);
            section.SetBlock(blockState, x, inSectionY, z);
        }

        public static sbyte GetBlockLight(this ChuckSection chuckSection, int blockPosition)
        {
            return Nibble4(chuckSection.BlockLight, blockPosition);
        }

        public static sbyte GetBlockLight(this ChuckSection chuckSection, int x, int y, int z)
        {
            return chuckSection.GetBlockLight(GetBlockPosition(x, y, z));
        }

        public static sbyte GetBlockLight(this ChuckData chuckData, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            return chuckData.GetSectionOrNull(sectionY)?
                       .GetBlockLight(x, inSectionY, z)
                   ?? 0;
        }

        public static void SetBlockLight(this ChuckSection chuckSection, sbyte light, int blockPosition)
        {
            Nibble4(chuckSection.BlockLight, blockPosition, light);
        }

        public static void SetBlockLight(this ChuckSection chuckSection, sbyte light, int x, int y, int z)
        {
            chuckSection.SetBlockLight(light, GetBlockPosition(x, y, z));
        }

        public static void SetBlockLight(this ChuckData chuckData, sbyte light, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            var section = chuckData.GetSectionOrCreate(sectionY);
            section.SetBlockLight(light, x, inSectionY, z);
        }

        public static sbyte GetSkyLight(this ChuckSection chuckSection, int blockPosition)
        {
            return Nibble4(chuckSection.SkyLight, blockPosition);
        }

        public static sbyte GetSkyLight(this ChuckSection chuckSection, int x, int y, int z)
        {
            return chuckSection.GetSkyLight(GetBlockPosition(x, y, z));
        }

        public static sbyte GetSkyLight(this ChuckData chuckData, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            return chuckData.GetSectionOrNull(sectionY)?
                       .GetSkyLight(x, inSectionY, z)
                   ?? 0;
        }

        public static void SetSkyLight(this ChuckSection chuckSection, sbyte light, int blockPosition)
        {
            Nibble4(chuckSection.SkyLight, blockPosition, light);
        }

        public static void SetSkyLight(this ChuckSection chuckSection, sbyte light, int x, int y, int z)
        {
            chuckSection.SetBlockLight(light, GetBlockPosition(x, y, z));
        }

        public static void SetSkyLight(this ChuckData chuckData, sbyte light, int x, int y, int z)
        {
            var sectionY = (sbyte) (y >> 4);
            var inSectionY = y & 0x0F;
            var section = chuckData.GetSectionOrCreate(sectionY);
            section.SetSkyLight(light, x, inSectionY, z);
        }
    }
}