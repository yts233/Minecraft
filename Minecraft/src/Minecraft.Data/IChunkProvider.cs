using System.Collections.Generic;

namespace Minecraft.Data
{
    public interface IChunkProvider
    {
        IEnumerable<IChunk> EnumerateChunks();
        IChunk GetChunk(int x, int z);
        bool HasChunk(int x, int z);
    }
}
