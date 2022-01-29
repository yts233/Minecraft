using System.Collections.Generic;

namespace Minecraft.Protocol.Client.Handlers
{
    public interface IWorldHandler
    {
        IReadOnlyCollection<IEntityHandler> GetEntities();
        IChunkHandler GetChunk(int x,int z);
    }
}
