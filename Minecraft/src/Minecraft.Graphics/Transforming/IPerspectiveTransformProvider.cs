namespace Minecraft.Graphics.Transforming
{
    public interface IPerspectiveTransformProvider : IProjectionTransformProvider
    {
        IEye Eye { get; set; }
    }
}