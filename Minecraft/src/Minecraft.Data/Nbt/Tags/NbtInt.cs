namespace Minecraft.Data.Nbt.Tags
{
    public class NbtInt : NbtValue
    {
        public NbtInt(int value) : base(value)
        {
        }

        public override NbtTagType Type => NbtTagType.Int;
    }
}