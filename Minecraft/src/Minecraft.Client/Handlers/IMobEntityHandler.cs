namespace Minecraft.Client.Handlers
{
    public interface IMobEntityHandler : IVelocityEntityHandler
    {
        int Type { get; }
    }
}
