using System;

namespace Minecraft.Data.Nbt.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NbtTagAttribute : Attribute
    {
        public string Name { get; set; } = null;

        public NbtTagType Type { get; set; }
    }
}