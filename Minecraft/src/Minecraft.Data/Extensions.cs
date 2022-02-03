using Minecraft.Data.Common.Blocking;
using System;

namespace Minecraft.Data
{
    public static class Extensions
    {
        /// <summary>
        /// Fill the blocks
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        /// <param name="block"></param>
        /// <returns>The count of filled blocks</returns>
        public static int Fill(this IBlockEditor editor, int x1, int y1, int z1, int x2, int y2, int z2, BlockState block)
        {
            int count = 0;
            int ix1 = Math.Min(x1, x2),
                ix2 = Math.Max(x1, x2),
                iy1 = Math.Min(y1, y2),
                iy2 = Math.Max(y1, y2),
                iz1 = Math.Min(z1, z2),
                iz2 = Math.Max(z1, z2);
            for (int y = iy1; y <= iy2; y++)
                for (int z = iz1; z <= iz2; z++)
                    for (int x = ix1; x <= ix2; x++)
                        if (editor.SetBlock(x, y, z, block))
                            count++;
            return count;
        }

        public static int FillFast(this IEditableWorld world, int x1, int y1, int z1, int x2, int y2, int z2, BlockState block)
        {
            int count = 0;
            int ix1 = Math.Min(x1, x2),
                ix2 = Math.Max(x1, x2),
                iy1 = Math.Min(y1, y2),
                iy2 = Math.Max(y1, y2),
                iz1 = Math.Min(z1, z2),
                iz2 = Math.Max(z1, z2);
            int cx1 = ix1 >> 4,
                cx2 = ix2 >> 4,
                cz1 = iz1 >> 4,
                cz2 = iz2 >> 4;
            for (int cz = cz1; cz <= cz2; cz++)
                for (int cx = cx1; cx <= cx2; cx++)
                {
                    if (!(world.GetChunk(cx, cz) is IBlockEditor chunk))
                        continue;

                    int bx1 = Math.Max(ix1 - (cx << 4), 0);
                    int bx2 = Math.Min(ix2 - (cx << 4), 15);
                    int bz1 = Math.Max(iz1 - (cz << 4), 0);
                    int bz2 = Math.Min(iz2 - (cz << 4), 15);
                    for (int y = iy1; y <= iy2; y++)
                        for (int z = bz1; z <= bz2; z++)
                            for (int x = bx1; x <= bx2; x++)
                                chunk.SetBlock(x, y, z, block);
                }

            return count;
        }
    }
}
