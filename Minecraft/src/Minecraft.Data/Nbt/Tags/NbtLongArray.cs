using System.Collections.Generic;

namespace Minecraft.Data.Nbt.Tags
{
    public class NbtLongArray : NbtArray<long>
    {
        public NbtLongArray(long[] array) : base(array)
        {
        }

        public override NbtTagType Type => NbtTagType.LongArray;
    }
}