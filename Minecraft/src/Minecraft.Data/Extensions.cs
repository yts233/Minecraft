using Minecraft.Data.Common.Blocking;

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
        public static int Fill(this IBlockEditor editor,int x1, int y1, int z1, int x2, int y2, int z2, BlockState block)
        {
            int count = 0;
            for (int y = y1; y <= y2; y++)
                for (int z = z1; z <= z2; z++)
                    for (int x = x1; x <= x2; x++)
                        if (editor.SetBlock(x, y, z, block)) 
                            count++;
            return count;
        }
    }
}
