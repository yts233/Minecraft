using Minecraft.Data.Nbt.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Kermalis.EndianBinaryIO;

namespace Minecraft.Data.Nbt
{
    public class NbtWriter : IDisposable
    {
        private readonly EndianBinaryWriter _writer;

        public NbtWriter(Stream baseStream)
        {
            _writer = new EndianBinaryWriter(new DevidedStream(baseStream), Endianness.BigEndian, EncodingType.UTF8, BooleanSize.U8);
        }

        public void WriteTag(NbtTag tag)
        {
            var name = tag.Name ?? "";
            _writer.Write((sbyte)tag.Type);
            _writer.Write(name.Length);
            _writer.Write(name.ToCharArray());
            WritePayload(tag);
        }

        private void WritePayload(NbtTag tag)
        {
            switch (tag.Type)
            {
                case NbtTagType.End:
                    return;
                case NbtTagType.Byte:
                    {
                        var value = (sbyte)((NbtValue)tag).Value;
                        _writer.Write(value);
                        return;
                    }
                case NbtTagType.Short:
                    {
                        var value = (short)((NbtValue)tag).Value;
                        _writer.Write(value);
                        return;
                    }
                case NbtTagType.Int:
                    {
                        var value = (int)((NbtValue)tag).Value;
                        _writer.Write(value);
                        return;
                    }
                case NbtTagType.Long:
                    {
                        var value = (long)((NbtValue)tag).Value;
                        _writer.Write(value);
                        return;
                    }
                case NbtTagType.Float:
                    {
                        var value = (float)((NbtValue)tag).Value;
                        _writer.Write(value);
                        return;
                    }
                case NbtTagType.Double:
                    {
                        var value = (double)((NbtValue)tag).Value;
                        _writer.Write(value);
                        return;
                    }
                case NbtTagType.ByteArray:
                    {
                        var value = ((NbtArray<sbyte>)tag).Value;
                        for (var i = 0; i < value.Length; i++)
                        {
                            _writer.Write(value[i]);
                        }
                        return;
                    }
                case NbtTagType.String:
                    {
                        var value = ((string)((NbtValue)tag).Value).ToCharArray();
                        _writer.Write(value);
                        return;
                    }
                case NbtTagType.List:
                    {
                        var value = (NbtList)tag;
                        _writer.Write((sbyte)value.ContentType);
                        _writer.Write(value.Count);
                        foreach (var v in value)
                        {
                            WritePayload(v);
                        }
                        return;
                    }
                case NbtTagType.Compound:
                    {
                        var value = (NbtCompound)tag;
                        foreach (var (_, v) in value)
                        {
                            WriteTag(v);
                        }
                        return;
                    }
                case NbtTagType.IntArray:
                    {
                        var value = ((NbtArray<int>)tag).Value;
                        for (var i = 0; i < value.Length; i++)
                        {
                            _writer.Write(value[i]);
                        }
                        return;
                    }
                case NbtTagType.LongArray:
                    {
                        var value = ((NbtArray<long>)tag).Value;
                        for (var i = 0; i < value.Length; i++)
                        {
                            _writer.Write(value[i]);
                        }
                        return;
                    }
                default:
                    throw new NbtException($"Unknow tag. Tag id: {(sbyte)tag.Type}");
            }
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
