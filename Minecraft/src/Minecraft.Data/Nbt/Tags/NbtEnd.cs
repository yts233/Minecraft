using System;

namespace Minecraft.Data.Nbt.Tags
{
    public class NbtEnd : NbtValue, IEquatable<NbtEnd>
    {
        public NbtEnd() : base(null)
        {
        }

        public override NbtTagType Type => NbtTagType.End;

        public bool Equals(NbtEnd other)
        {
            return true;
        }

        public override bool Equals(NbtTag other)
        {
            return other is NbtEnd;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}