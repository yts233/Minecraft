namespace Minecraft.Graphics.Transforming
{
    public static class Extensions
    {
        public static IViewTransformProvider GetViewTransformProvider(this ICamera camera)
        {
            return new ViewTransformProvider(camera);
        }

        public static IPerspectiveTransformProvider GetPerspectiveTransformProvider(this IEye eye)
        {
            return new PerspectiveTransformProvider(eye);
        }
    }
}