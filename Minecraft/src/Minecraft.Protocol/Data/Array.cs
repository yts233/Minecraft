using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Minecraft.Protocol.Data
{
    /// <summary>
    /// Zero or more fields of type <see cref="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>The count must be known from the context. </remarks>
    public class Array<T> : IDataType<T[]> where T : IDataType, new()
    {
        private T[] _value;
        public T[] Value => _value;

        public Array()
        {
        }

        public Array(T[] array)
        {
            _value = array;
            Length = array.Length;
        }

        public int Length { get; set; } = 0;

        public void ReadFromStream(Stream stream)
        {
            var content = this.GetContent(stream);
            lock (_value = new T[Length])
            {
                for (var i = 0; i < _value.Length; i++)
                {
                    _value[i] = content.Read<T>();
                }
            }
        }

        public void WriteToStream(Stream stream)
        {
            var content = this.GetContent(stream);
            lock (_value)
            {
                for(var i = 0; i < _value.Length; i++)
                {
                    content.Write(_value[0]);
                }
            }
        }
    }
}
