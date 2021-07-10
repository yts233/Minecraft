namespace Minecraft.Data.Nbt.Tags
{
    public class NbtShort : NbtValue
    {
        public NbtShort(short value) : base(value)
        {
        }

        public override NbtTagType Type => NbtTagType.Short;
    }
}