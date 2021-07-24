using System.Collections.Generic;
using Minecraft.Data.Nbt;

namespace Minecraft.Data.Nbt.Tags
{
    public class NbtIntArray : NbtArray<int>
    {
        public NbtIntArray(int[] array) : base(array)
        {
        }

        public override NbtTagType Type => NbtTagType.IntArray;
    }
}