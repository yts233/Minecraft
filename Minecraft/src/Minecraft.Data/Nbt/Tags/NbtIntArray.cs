using System.Collections.Generic;
using Minecraft.Data.Nbt;

namespace Test.Data.Nbt.Test.Nbt.Tags
{
    public class NbtIntArray : NbtArray<int>
    {
        public NbtIntArray()
        {
        }

        public NbtIntArray(IEnumerable<int> enumerable)
        {
            foreach (var @int in enumerable)
                Add(@int);
        }

        public override NbtTagType Type => NbtTagType.IntArray;
    }
}