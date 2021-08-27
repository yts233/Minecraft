using System.Collections.Generic;

namespace Minecraft.Client.Handlers
{
    public interface IWorldHandler
    {
        IReadOnlyCollection<IEntityHandler> GetEntities();
    }
}
