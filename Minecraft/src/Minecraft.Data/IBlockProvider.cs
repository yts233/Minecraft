using Minecraft.Data.Common.Blocking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Data
{
    public interface IBlockProvider
    {
        /// <summary>
        /// Get the block
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        BlockState GetBlock(int x, int y, int z);
        /// <summary>
        /// Enumerate the blocks
        /// </summary>
        /// <remarks>no air</remarks>
        /// <returns></returns>
        IEnumerable<(int x, int y, int z, BlockState block)> EnumerateBlocks();
        bool IsTile(int x, int y, int z);
#if false
        /// <summary>
        /// Enumerate the range
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        /// <remarks>no air</remarks>
        /// <returns></returns>
        IEnumerable<(int x, int y, int z, BlockState block)> EnumerateRange(int x1, int y1, int z1, int x2, int y2, int z2);
        /// <summary>
        /// Enumerate the chunk
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <remarks>no air</remarks>
        /// <returns></returns>
        IEnumerable<(int x, int y, int z, BlockState block)> EnumerateChunk(int x, int z);
        /// <summary>
        /// Enumerate the chunk section
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <remarks>no air</remarks>
        /// <returns></returns>
        IEnumerable<(int x, int y, int z, BlockState block)> EnumerateChunkSection(int x, int y, int z);
#endif
    }
}
