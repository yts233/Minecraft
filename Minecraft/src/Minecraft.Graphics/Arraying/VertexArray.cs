using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL;

namespace Minecraft.Graphics.Arraying
{
    /// <summary>
    ///     顶点堆
    /// </summary>
    public class VertexArray<T> : IVertexArray, IEnumerable<T> where T : struct
    {
        private readonly int _count;

        private readonly int _vertexArrayObject;
        private readonly int _vertexBufferObject;
        private readonly T[] _vertices;

        /// <summary>
        ///     创建顶点堆
        /// </summary>
        /// <param name="vertices">顶点</param>
        /// <param name="pointers">定点指针属性</param>
        public VertexArray(IEnumerable<T> vertices, IEnumerable<VertexAttributePointer> pointers)
        {
            static int GetSize(VertexAttribPointerType type)
            {
                return type switch
                {
                    VertexAttribPointerType.Byte => sizeof(sbyte),
                    VertexAttribPointerType.Double => sizeof(double),
                    VertexAttribPointerType.UnsignedByte => sizeof(byte),
                    VertexAttribPointerType.Short => sizeof(short),
                    VertexAttribPointerType.UnsignedShort => sizeof(ushort),
                    VertexAttribPointerType.Int => sizeof(int),
                    VertexAttribPointerType.UnsignedInt => sizeof(uint),
                    VertexAttribPointerType.Float => sizeof(float),
                    VertexAttribPointerType.HalfFloat => sizeof(float) / 2,
                    VertexAttribPointerType.Fixed => throw new NotSupportedException(),
                    VertexAttribPointerType.UnsignedInt2101010Rev => throw new NotSupportedException(),
                    VertexAttribPointerType.UnsignedInt10F11F11FRev => throw new NotSupportedException(),
                    VertexAttribPointerType.Int2101010Rev => throw new NotSupportedException(),
                    _ => throw new ArgumentOutOfRangeException(nameof(type))
                };
            }

            _vertices = vertices.ToArray();
            _vertexBufferObject = GL.GenBuffer();
            _vertexArrayObject = GL.GenVertexArray();
            var size = Marshal.SizeOf(typeof(T));
            var totalSize = _vertices.Length * size;
            var vertexAttributePointers = pointers as VertexAttributePointer[] ?? pointers.ToArray();
            var stride = vertexAttributePointers
                .Select(pointer => pointer.Offset + pointer.Size * GetSize((VertexAttribPointerType) pointer.Type))
                .Prepend(0).Max();
            _count = _vertices.Length * size / stride;
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, totalSize, _vertices,
                BufferUsageHint.StaticDraw);
            Bind();
            foreach (var pointer in vertexAttributePointers)
            {
                GL.VertexAttribPointer(
                    pointer.Index,
                    pointer.Size,
                    (VertexAttribPointerType) pointer.Type,
                    pointer.Normalized,
                    stride,
                    pointer.Offset);
                GL.EnableVertexAttribArray(pointer.Index);
            }

            Logger.Info<VertexArray<T>>($"Create an vertex array:{_vertexArrayObject}");
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>) _vertices).GetEnumerator();
        }

        int IHandle.Handle => _vertexArrayObject;

        /// <summary>
        ///     渲染
        /// </summary>
        public void Render()
        {
            Render(0, _count);
        }

        /// <summary>
        ///     渲染
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="count">数量</param>
        public void Render(int index, int count)
        {
            Bind();
            GL.DrawArrays(PrimitiveType.Triangles, index, count);
        }

        public void Bind()
        {
            GL.BindVertexArray(_vertexArrayObject);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<byte>) this).GetEnumerator();
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteBuffer(_vertexBufferObject);
        }

        /// <summary>
        ///     获取不包含原数组的顶点堆
        /// </summary>
        /// <remarks>理论上会减少资源占用</remarks>
        /// <returns>顶点堆的Handle</returns>
        public IVertexArrayHandle GetHandle()
        {
            return new VertexArrayHandle(_vertexArrayObject, _vertexBufferObject, _count);
        }

        private class VertexArrayHandle : IVertexArrayHandle
        {
            private readonly int _count;
            private readonly int _vbo;
            private readonly int _vao;

            public VertexArrayHandle(int handle, int vertexBufferObjectHandle, int count)
            {
                _vao = handle;
                _vbo = vertexBufferObjectHandle;
                _count = count;
            }

            int IVertexArrayHandle.VertexArrayObjectHandle => _vao;
            int IVertexArrayHandle.VertexBufferObjectHandle => _vbo;
            int IVertexArrayHandle.Count => _count;
        }
    }
}