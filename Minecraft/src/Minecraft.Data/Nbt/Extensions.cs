using System;
using System.Collections;
using System.IO;
using System.Text;
using Minecraft.Data.Nbt.Tags;
using Test.Data.Nbt.Test.Nbt.Tags;

namespace Minecraft.Data.Nbt
{
    public static class Extensions
    {
        public static string ToStringedNbt(this NbtTag tag, bool format = false)
        {
            var builder = new StringBuilder();
            var writer = new SNbtWriter(new StringWriter(builder), format);

            void CheckTag(NbtTag nbtTag)
            {
                var value = nbtTag as NbtValue;
                var needClose = false;
                switch (nbtTag.Type)
                {
                    case NbtTagType.End:
                        break;
                    case NbtTagType.Byte:
                        writer.WriteByteTag((sbyte) value!.Value, nbtTag.Name);
                        break;
                    case NbtTagType.Short:
                        writer.WriteShortTag((short) value!.Value, nbtTag.Name);
                        break;
                    case NbtTagType.Int:
                        writer.WriteIntTag((int) value!.Value, nbtTag.Name);
                        break;
                    case NbtTagType.Long:
                        writer.WriteLongTag((long) value!.Value, nbtTag.Name);
                        break;
                    case NbtTagType.Float:
                        writer.WriteFloatTag((float) value!.Value, nbtTag.Name);
                        break;
                    case NbtTagType.Double:
                        writer.WriteDoubleTag((double) value!.Value, nbtTag.Name);
                        break;
                    case NbtTagType.ByteArray:
                        writer.WriteByteArrayTag((NbtByteArray) nbtTag, nbtTag.Name);
                        break;
                    case NbtTagType.String:
                        writer.WriteStringTag((string) value!.Value, nbtTag.Name);
                        break;
                    case NbtTagType.List:
                        writer.WriteListTagStart(nbtTag.Name);
                        needClose = true;
                        break;
                    case NbtTagType.Compound:
                        writer.WriteCompoundTagStart(nbtTag.Name);
                        needClose = true;
                        break;
                    case NbtTagType.IntArray:
                        writer.WriteIntArrayTag((NbtIntArray) nbtTag, nbtTag.Name);
                        break;
                    case NbtTagType.LongArray:
                        writer.WriteLongArrayTag((NbtLongArray) nbtTag, nbtTag.Name);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(nbtTag));
                }

                foreach (var children in nbtTag)
                    CheckTag(children);

                if (needClose)
                    writer.WriteEndTag();
            }

            CheckTag(tag);

            return builder.ToString();
        }

        public static NbtCompound ToNbtCompound(this IDictionary dictionary)
        {
            var nbt = new NbtCompound();
            foreach (DictionaryEntry dictionaryEntry in dictionary)
                nbt.Add(NbtValue.CreateValue(dictionaryEntry.Value), (string) dictionaryEntry.Key);
            return nbt;
        }
    }
}