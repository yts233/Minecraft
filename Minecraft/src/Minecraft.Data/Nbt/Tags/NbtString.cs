namespace Minecraft.Data.Nbt.Tags
{
    public class NbtString : NbtValue
    {
        public NbtString(string value) : base(value)
        {
        }

        public override NbtTagType Type => NbtTagType.String;
    }
}