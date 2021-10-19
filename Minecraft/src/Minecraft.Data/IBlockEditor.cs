using Minecraft.Data.Common.Blocking;

namespace Minecraft.Data
{
    public interface IBlockEditor
    {
        /// <summary>
        /// Set the block
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        bool SetBlock(int x, int y, int z, BlockState block);
#if false
        /// <summary>
        /// Fill the blocks
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        /// <param name="block"></param>
        /// <returns>The count of filled blocks</returns>
        int FillBlocks(int x1, int y1, int z1, int x2, int y2, int z2, BlockState block);
#endif
    }
}
