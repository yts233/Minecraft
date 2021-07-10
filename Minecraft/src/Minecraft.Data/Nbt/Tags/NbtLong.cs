namespace Minecraft.Data.Nbt.Tags
{
    public class NbtLong : NbtValue
    {
        public NbtLong(long value) : base(value)
        {
        }

        public override NbtTagType Type => NbtTagType.Long;
    }
}