using System;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    /// 枚举
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEnum<out T> : IDataType<T> where T : Enum
    {
    }
}