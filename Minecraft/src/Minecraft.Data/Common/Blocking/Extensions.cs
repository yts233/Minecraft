namespace Minecraft.Data.Common.Blocking
{
    public static class Extensions
    {
#nullable enable
        public static bool IsAir(this BlockState? block)
        {
            return block == null || block.Name.Equals("air") || block.Name.Equals("void_air") || block.Name.Equals("cave_air");
        }
#nullable disable
    }
}