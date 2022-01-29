using Minecraft.Numerics;
using System.Threading.Tasks;

namespace Minecraft.Protocol.Client.Handlers
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

        void Attack(IEntityHandler entity, bool sneaking);
        void Interact(IEntityHandler entity, Hand hand, bool sneaking);
        void Interact(IEntityHandler entity, Vector3f target, Hand hand, bool sneaking);
    }
}
