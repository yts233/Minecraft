using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Minecraft.Extensions;

namespace Minecraft.Data.Nbt
{
    public class SNbtWriter
    {
        private readonly Stack<int> _children = new Stack<int>(new[] {0});
        private readonly bool _format;
        private readonly Stack<NbtTagType> _node = new Stack<NbtTagType>();
        private readonly TextWriter _writer;

        public SNbtWriter(TextWriter writer, bool format = false)
        {
            _writer = writer;
            _format = format;
        }

        private void WriteSpace()
        {
            _writer.Write('\n');
            if (_format)
                _writer.Write(string.Empty.PadLeft(_node.Count * 2));
        }

        public void WriteEndTag()
        {
            if (_node.Count == 0) return;
            switch (_node.Peek())
            {
                case NbtTagType.Compound:
                    _node.Pop();
                    WriteSpace();
                    _writer.Write("}");
                    break;
                case NbtTagType.List:
                    _node.Pop();
                    WriteSpace();
                    _writer.Write("]");
                    break;
                case NbtTagType.ByteArray:
                    break;
                case NbtTagType.IntArray:
                    break;
                case NbtTagType.LongArray:
                    break;
                case NbtTagType.End:
                    break;
                case NbtTagType.Byte:
                    break;
                case NbtTagType.Short:
                    break;
                case NbtTagType.Int:
                    break;
                case NbtTagType.Long:
                    break;
                case NbtTagType.Float:
                    break;
                case NbtTagType.Double:
                    break;
                case NbtTagType.String:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WriteTagStart(string name)
        {
            if (_children.Peek() != 0)
                _writer.Write(_format ? ", " : ",");
            if (_node.Count != 0)
                WriteSpace();
            if (name != null)
                _writer.Write(_format ? $"{name}: " : $"{name}:");
            _children.Push(_children.Pop() + 1);
        }

        private void WriteValue(object value, string suffix)
        {
            _writer.Write($"{value}{suffix}");
        }

        private void WriteArray(ICollection list, string prefix)
        {
            _writer.Write(_format ? $"[{prefix}; " : $"[{prefix};");
            var i = 0;
            foreach (var obj in list)
            {
                _writer.Write(obj);
                if (++i != list.Count) _writer.Write(_format ? ", " : ",");
            }

            _writer.Write("]");
        }

        public void WriteByteTag(sbyte value, string name = null)
        {
            WriteTagStart(name);
            WriteValue(value, "b");
        }

        public void WriteShortTag(short value, string name = null)
        {
            WriteTagStart(name);
            WriteValue(value, "s");
        }

        public void WriteIntTag(int value, string name = null)
        {
            WriteTagStart(name);
            WriteValue(value, "");
        }

        public void WriteLongTag(long value, string name = null)
        {
            WriteTagStart(name);
            WriteValue(value, "l");
        }

        public void WriteFloatTag(float value, string name = null)
        {
            WriteTagStart(name);
            WriteValue(value, "f");
        }

        public void WriteDoubleTag(double value, string name = null)
        {
            WriteTagStart(name);
            WriteValue(value, "d");
        }

        public void WriteByteArrayTag(IEnumerable<sbyte> value, string name = null)
        {
            WriteTagStart(name);
            WriteArray(value.ToArray(), "b");
        }

        public void WriteStringTag(string value, string name = null)
        {
            WriteTagStart(name);
            _writer.Write($"\"{value.ToEscape()}\"");
        }

        public void WriteListTagStart(string name = null)
        {
            WriteTagStart(name);
            _node.Push(NbtTagType.List);
            _children.Push(0);
            _writer.Write('[');
        }

        public void WriteCompoundTagStart(string name = null)
        {
            WriteTagStart(name);
            _node.Push(NbtTagType.Compound);
            _children.Push(0);
            _writer.Write('{');
        }

        public void WriteIntArrayTag(IEnumerable<int> value, string name = null)
        {
            WriteTagStart(name);
            WriteArray(value.ToArray(), "i");
        }

        public void WriteLongArrayTag(IEnumerable<long> value, string name = null)
        {
            WriteTagStart(name);
            WriteArray(value.ToArray(), "l");
        }
    }
}