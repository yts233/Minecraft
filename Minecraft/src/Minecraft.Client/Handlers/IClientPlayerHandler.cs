namespace Minecraft.Client.Handlers
{
    public interface IClientPlayerHandler : IPlayerHandler
    {
        new IControlablePositionHandler GetPositionHandler();
        IPositionHandler IEntityHandler.GetPositionHandler()
        {
            return GetPositionHandler();
        }
    }
}
