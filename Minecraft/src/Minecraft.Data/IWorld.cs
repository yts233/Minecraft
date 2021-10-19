namespace Minecraft.Data
{
    public interface IWorld : IBlockProvider
    {
        /// <summary>
        /// Get the chunk
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        IChunk GetChunk(int x, int z);
        bool HasChunk(int x, int z);
        bool AddChunk(int x, int z, IChunk chunk);
        bool RemoveChunk(int x, int z);
    }
}
