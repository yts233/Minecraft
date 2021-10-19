namespace Minecraft.Data
{
    public interface IChunk : IBlockProvider
    {
        int X { get; }
        int Z { get; }
        /// <summary>
        /// If no non-air block
        /// </summary>
        bool IsEmpty { get; }
        /// <summary>
        /// The count of non-air blocks
        /// </summary>
        int BlockCount { get; }
    }
}
