namespace Minecraft.Graphics.Transforming
{
    public interface IEye : ICamera
    {
        public float FovY { get; set; }
        float Aspect { get; set; }
        float DepthNear { get; set; }
        float DepthFar { get; set; }
    }
}