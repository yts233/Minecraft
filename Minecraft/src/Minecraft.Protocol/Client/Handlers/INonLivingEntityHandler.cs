namespace Minecraft.Protocol.Client.Handlers
{
    public interface INonLivingEntityHandler : IMobEntityHandler
    {
        int Data { get; }
    }
}
