using System.Collections.Generic;

namespace Minecraft.Data.Nbt.Tags
{
    public class NbtByteArray : NbtArray<sbyte>
    {
        public NbtByteArray()
        {
        }

        public NbtByteArray(IEnumerable<sbyte> enumerable)
        {
            foreach (var @sbyte in enumerable)
                Add(@sbyte);
        }

        public override NbtTagType Type => NbtTagType.ByteArray;
    }
}