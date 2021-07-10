using System;
using System.Collections.Generic;
using Minecraft.Data.Nbt.Tags;

namespace Minecraft.Data.Nbt
{
    public abstract class NbtValue : NbtTag, IEquatable<NbtValue>
    {
        protected NbtValue(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public abstract override NbtTagType Type { get; }

        public override int Count => throw new NotSupportedException();

        public override bool IsReadOnly => true;

        public bool Equals(NbtValue other)
        {
            return other?.Value == Value;
        }

        public static NbtValue CreateValue(object value)
        {
            return value switch
            {
                sbyte v => new NbtByte(v),
                short v => new NbtShort(v),
                int v => new NbtInt(v),
                long v => new NbtLong(v),
                float v => new NbtFloat(v),
                string v => new NbtString(v),
                _ => throw new InvalidOperationException()
            };
        }

        protected override IEnumerator<NbtTag> GetChildrenTags()
        {
            yield break;
        }

        protected override bool _Add(NbtTag item)
        {
            throw new NotSupportedException();
        }

        protected override void _Clear()
        {
            throw new NotSupportedException();
        }

        public override bool Contains(NbtTag item)
        {
            throw new NotSupportedException();
        }

        protected override bool _Remove(NbtTag item)
        {
            throw new NotSupportedException();
        }

        public override void CopyTo(NbtTag[] array, int arrayIndex)
        {
            array[arrayIndex] = this;
        }

        public override bool Equals(NbtTag other)
        {
            return other is NbtValue value && Equals(value);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }
    }
}