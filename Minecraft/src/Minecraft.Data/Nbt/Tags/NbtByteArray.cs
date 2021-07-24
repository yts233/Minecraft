using System.Collections.Generic;

namespace Minecraft.Data.Nbt.Tags
{
    public class NbtByteArray : NbtArray<sbyte>
    {
        public NbtByteArray(sbyte[] array) : base(array)
        {
        }

        public override NbtTagType Type => NbtTagType.ByteArray;
    }
}