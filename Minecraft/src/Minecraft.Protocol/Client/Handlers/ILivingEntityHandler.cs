namespace Minecraft.Protocol.Client.Handlers
{
    public interface ILivingEntityHandler : IMobEntityHandler
    {
        float HeadPitch { get; }
    }
}
