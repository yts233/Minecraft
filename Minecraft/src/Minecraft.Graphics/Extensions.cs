using OpenTK.Mathematics;

namespace Minecraft.Graphics
{
    public static class Extensions
    {
        public static Vector2 FromBox(this Vector2 vec, Box2 box)
        {
            vec *= box.Size;
            vec += box.Min;
            return vec;
        }
    }
}
