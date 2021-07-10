namespace Minecraft.Data.Nbt.Tags
{
    public class NbtByte : NbtValue
    {
        public NbtByte(sbyte value) : base(value)
        {
        }

        public override NbtTagType Type => NbtTagType.Byte;
    }
}