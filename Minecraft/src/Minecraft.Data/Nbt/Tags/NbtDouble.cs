namespace Minecraft.Data.Nbt.Tags
{
    public class NbtDouble : NbtValue
    {
        public NbtDouble(double value) : base(value)
        {
        }

        public override NbtTagType Type => NbtTagType.Double;
    }
}