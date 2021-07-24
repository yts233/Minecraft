using Kermalis.EndianBinaryIO;
using Minecraft.Data.Nbt.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Minecraft.Data.Nbt
{
    public class NbtReader : IDisposable
    {
        private readonly EndianBinaryReader _reader;
        public NbtReader(Stream baseStream)
        {
            _reader = new EndianBinaryReader(new DevidedStream(baseStream), Endianness.BigEndian, EncodingType.UTF8, BooleanSize.U8);
        }

        private NbtTag ReadPayload(NbtTagType nbtType)
        {
            switch (nbtType)
            {
                case NbtTagType.Byte:
                    return new NbtByte(_reader.ReadSByte());
                case NbtTagType.Short:
                    return new NbtShort(_reader.ReadInt16());
                case NbtTagType.Int:
                    return new NbtInt(_reader.ReadInt32());
                case NbtTagType.Long:
                    return new NbtLong(_reader.ReadInt64());
                case NbtTagType.Float:
                    return new NbtFloat(_reader.ReadSingle());
                case NbtTagType.Double:
                    return new NbtDouble(_reader.ReadDouble());
                case NbtTagType.ByteArray:
                    {
                        var size = _reader.ReadInt32();
                        var array = new sbyte[size];
                        for (var i = 0; i < size; i++)
                        {
                            array[i] = _reader.ReadSByte();
                        }
                        return new NbtByteArray(array);
                    }
                case NbtTagType.String:
                    return new NbtString(new string(_reader.ReadChars(_reader.ReadUInt16(), false)));
                case NbtTagType.List:
                    {
                        var contentType = (NbtTagType)_reader.ReadByte();
                        var size = _reader.ReadInt32();
                        var list = new NbtList();
                        for (var i = 0; i < size; i++)
                        {
                            list.Add(ReadPayload(contentType));
                        }
                        return list;
                    }

                case NbtTagType.Compound:
                    {
                        var compound = new NbtCompound();
                        NbtTag tag;
                        while ((tag = ReadTag()).Type != NbtTagType.End)
                        {
                            compound.Add(tag);
                        }
                        return compound;
                    }
                case NbtTagType.IntArray:
                    {
                        var size = _reader.ReadInt32();
                        var array = new int[size];
                        for (var i = 0; i < size; i++)
                        {
                            array[i] = _reader.ReadInt32();
                        }
                        return new NbtIntArray(array);
                    }
                case NbtTagType.LongArray:
                    {
                        var size = _reader.ReadInt32();
                        var array = new long[size];
                        for (var i = 0; i < size; i++)
                        {
                            array[i] = _reader.ReadInt64();
                        }
                        return new NbtLongArray(array);
                    }
                default:
                    throw new NbtException($"Unknow tag. Tag id: {(sbyte)nbtType}");
            }
        }

        public NbtTag ReadTag()
        {
            lock (_reader)
            {
                var nbtType = (NbtTagType)_reader.ReadByte();
                if (nbtType == NbtTagType.End)
                    return new NbtEnd();
                var nameLength = (ushort)_reader.ReadInt16();
                var name = new string(_reader.ReadChars(nameLength, false));
                var tag = ReadPayload(nbtType);
                tag.Name = name;
                return tag;
            }
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
