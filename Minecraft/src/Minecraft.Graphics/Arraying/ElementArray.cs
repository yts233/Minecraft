using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace Minecraft.Graphics.Arraying
{
    /// <summary>
    /// 元素堆
    /// </summary>
    public class ElementArray : IVertexArray, IEnumerable<uint>
    {
        private readonly int _count;
        private readonly int _elementBufferObject;
        private readonly uint[] _elements;

        private static Logger<ElementArray> _logger = Logger.GetLogger<ElementArray>();

        /// <summary>
        /// 顶点堆
        /// </summary>
        private readonly IVertexArrayHandle _vertexArrayHandle;

        /// <summary>
        /// 创建元素堆
        /// </summary>
        /// <param name="vertexArray">顶点堆</param>
        /// <param name="elements">元素堆</param>
        public ElementArray(IVertexArrayHandle vertexArray, IEnumerable<uint> elements)
        {
            _elements = elements.ToArray();
            _count = _elements.Length;
            _vertexArrayHandle = vertexArray;
            _vertexArrayHandle.Bind();
            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _elements.Length * sizeof(uint), _elements,
                BufferUsageHint.StaticDraw);
            _logger.Info($"Create an element array:{_elementBufferObject}");
        }

        IEnumerator<uint> IEnumerable<uint>.GetEnumerator()
        {
            return _elements.ToList().GetEnumerator();
        }

        int IHandle.Handle => _vertexArrayHandle.Handle;

        /// <summary>
        /// 绑定此顶点堆
        /// </summary>
        public void Bind()
        {
            _vertexArrayHandle.Bind();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<uint>) this).GetEnumerator();
        }

        /// <summary>
        /// 渲染
        /// </summary>
        public void Render()
        {
            Render(0, _count);
        }

        /// <summary>
        /// 渲染
        /// </summary>
        /// <param name="index">起始元素</param>
        /// <param name="count">元素数</param>
        public void Render(int index, int count)
        {
            Bind();
            GL.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, index * sizeof(uint));
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_elementBufferObject);
        }

        /// <summary>
        /// 获取不包含原数组的元素堆
        /// </summary>
        /// <remarks>理论上会减少资源占用</remarks>
        /// <returns>元素堆的Handle</returns>
        public IElementArrayHandle GetHandle()
        {
            return new ElementArrayHandle(_elementBufferObject, _vertexArrayHandle.VertexArrayObjectHandle,
                _vertexArrayHandle.VertexBufferObjectHandle, _count, _vertexArrayHandle.Count);
        }

        private readonly struct ElementArrayHandle : IElementArrayHandle
        {
            private readonly int _vertexCount;
            private readonly int _count;
            private readonly int _ebo;
            private readonly int _vbo;
            private readonly int _vao;

            public ElementArrayHandle(int handle, int vao, int vbo, int count, int vertexCount)
            {
                _ebo = handle;
                _vao = vao;
                _vbo = vbo;
                _count = count;
                _vertexCount = vertexCount;
            }
            
            int IElementArrayHandle.Count => _count;
            int IVertexArrayHandle.Count => _vertexCount;
            int IElementArrayHandle.ElementBufferObjectHandle => _ebo;
            int IVertexArrayHandle.VertexBufferObjectHandle => _vbo;
            int IVertexArrayHandle.VertexArrayObjectHandle => _vao;
        }
    }
}