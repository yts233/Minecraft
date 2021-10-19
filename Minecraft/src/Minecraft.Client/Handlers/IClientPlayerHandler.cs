using Minecraft.Numerics;
using System.Threading.Tasks;

namespace Minecraft.Client.Handlers
{
    public interface IClientPlayerHandler : IPlayerHandler
    {
        int ServerChunkX { get; }
        int ServerChunkZ { get; }

        new IControlablePositionHandler GetPositionHandler();
        IPositionHandler IEntityHandler.GetPositionHandler()
        {
            return GetPositionHandler();
        }

        Task Attack(IEntityHandler entity, bool sneaking);
        Task Interact(IEntityHandler entity, Hand hand, bool sneaking);
        Task Interact(IEntityHandler entity, Vector3f target, Hand hand, bool sneaking);
    }
}
