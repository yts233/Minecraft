using System;

namespace Minecraft.Data.Nbt.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class NbtCompoundAttribute : Attribute
    {
    }
}