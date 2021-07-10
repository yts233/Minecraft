namespace Minecraft.Extensions
{
    public static class NumbersExtension
    {
        public static int GetBitsCount(this int i)
        {
            var count = 0;
            while (i != 0)
            {
                count++;
                i >>= 1;
            }

            return count;
        }
    }
}