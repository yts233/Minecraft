namespace Minecraft.Data.Nbt.Tags
{
    public class NbtFloat : NbtValue
    {
        public NbtFloat(float value) : base(value)
        {
        }

        public override NbtTagType Type => NbtTagType.Float;
    }
}