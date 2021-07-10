using System.Collections.Generic;

namespace Minecraft.Data.Nbt.Tags
{
    public class NbtLongArray : NbtArray<long>
    {
        public NbtLongArray()
        {
        }

        public NbtLongArray(IEnumerable<long> enumerable)
        {
            foreach (var @long in enumerable)
                Add(@long);
        }

        public override NbtTagType Type => NbtTagType.LongArray;
    }
}