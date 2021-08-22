using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Minecraft.Graphics.Arraying
{
    public static class Extensions
    {
        /// <summary>
        ///     将当前的顶点堆转换为元素堆
        /// </summary>
        /// <param name="array">顶点堆</param>
        /// <param name="elements">元素</param>
        /// <returns></returns>
        public static ElementArray ToElementArray<T>(this VertexArray<T> array, IEnumerable<uint> elements)
            where T : struct
        {
            return new ElementArray(array.GetHandle(), elements);
        }

        public static VertexArray<T> ToVertexArray<T>(this IVertexArrayProvider<T> vertexArrayProvider) where T : struct
        {
            return new VertexArray<T>(vertexArrayProvider.GetVertices(), vertexArrayProvider.GetPointers());
        }

        public static ElementArray ToElementArray<T>(this IVertexArrayProvider<T> vertexArrayProvider)
            where T : struct
        {
            return vertexArrayProvider
                .ToVertexArray()
                .ToElementArray(vertexArrayProvider.GetIndices());
        }

        /// <summary>
        /// 修改顶点数据
        /// </summary>
        /// <param name="handle">顶点Handle</param>
        /// <param name="offset">字节偏移量</param>
        /// <param name="size">字节数量</param>
        /// <param name="data">字节数据</param>
        /// <remarks>请确保VertexBufferObjectHandle被绑定</remarks>
        /// <returns></returns>
        public static IVertexArrayHandle VertexSubData<T>(this IVertexArrayHandle handle, int offset, int size,
            T[] data) where T : struct
        {
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr) offset, size, data);
            return handle;
        }

        /// <summary>
        /// 修改元素数据
        /// </summary>
        /// <param name="handle">元素Handle</param>
        /// <param name="offset">字节偏移量</param>
        /// <param name="size">字节数量</param>
        /// <param name="data">字节数据</param>
        /// <remarks>请确保ElementBufferObjectHandle被绑定</remarks>
        /// <returns></returns>
        public static IElementArrayHandle ElementSubData(this IElementArrayHandle handle, int offset, int size,
            uint[] data)
        {
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr) offset, size, data);
            return handle;
        }

        public static IVertexArrayHandle BindVertexBufferObject(this IVertexArrayHandle handle)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle.VertexBufferObjectHandle);
            return handle;
        }

        public static IElementArrayHandle BindElementBufferObject(this IElementArrayHandle handle)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle.ElementBufferObjectHandle);
            return handle;
        }
    }
}