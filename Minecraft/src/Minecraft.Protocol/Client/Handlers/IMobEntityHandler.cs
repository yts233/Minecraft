namespace Minecraft.Protocol.Client.Handlers
{
    public interface IMobEntityHandler : IVelocityEntityHandler
    {
        int Type { get; }
    }
}
