using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Minecraft.Data.Nbt.Tags;
using Test.Data.Nbt.Test.Nbt.Tags;

namespace Minecraft.Data.Nbt.Serialization
{
    public static class NbtSerializer
    {
        public static NbtTag Serialize(object obj)
        {
            switch (obj)
            {
                case IDictionary dictionary:
                    return dictionary.ToNbtCompound();
                case IEnumerable<sbyte> enumerable:
                    return new NbtByteArray(enumerable);
                case IEnumerable<int> enumerable:
                    return new NbtIntArray(enumerable);
                case IEnumerable<long> enumerable:
                    return new NbtLongArray(enumerable);
                case IEnumerable enumerable:
                {
                    var list = new NbtList();
                    foreach (var obj1 in enumerable)
                        list.Add(Serialize(obj1));
                    return list;
                }
            }

            var type = obj.GetType();
            var tag = new NbtCompound();

            if (!type.GetCustomAttributes().Any(attribute => attribute is NbtCompoundAttribute))
                throw new SerializationException($"the object must inherit {typeof(NbtCompoundAttribute)}");

            foreach (var property in type.GetProperties())
            {
                var value = property.GetValue(obj);
                if (value == null)
                    continue;
                var attribute = property.GetCustomAttribute<NbtTagAttribute>();
                if (attribute == null)
                    continue;
                var name = attribute.Name;
                switch (attribute.Type)
                {
                    case NbtTagType.End:
                        break;
                    case NbtTagType.Byte:
                        tag.Add(NbtValue.CreateValue(value), name);
                        break;
                    case NbtTagType.Short:
                        tag.Add(NbtValue.CreateValue(value), name);
                        break;
                    case NbtTagType.Int:
                        tag.Add(NbtValue.CreateValue(value), name);
                        break;
                    case NbtTagType.Long:
                        tag.Add(NbtValue.CreateValue(value), name);
                        break;
                    case NbtTagType.Float:
                        tag.Add(NbtValue.CreateValue(value), name);
                        break;
                    case NbtTagType.Double:
                        tag.Add(NbtValue.CreateValue(value), name);
                        break;
                    case NbtTagType.ByteArray:
                        tag.Add(new NbtByteArray((IEnumerable<sbyte>) value), name);
                        break;
                    case NbtTagType.String:
                        tag.Add(NbtValue.CreateValue(value), name);
                        break;
                    case NbtTagType.List:
                        tag.Add(Serialize(value), name);
                        break;
                    case NbtTagType.Compound:
                        tag.Add(Serialize(value), name);
                        break;
                    case NbtTagType.IntArray:
                        tag.Add(new NbtIntArray((IEnumerable<int>) value), name);
                        break;
                    case NbtTagType.LongArray:
                        tag.Add(new NbtLongArray((IEnumerable<long>) value), name);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return tag;
        }
    }
}