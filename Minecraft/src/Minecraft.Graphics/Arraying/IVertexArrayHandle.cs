using System;
using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace Minecraft.Graphics.Arraying
{
    public interface IVertexArrayHandle : IHandle, IRenderable, IDisposable
    {
        int Count { get; }
        int VertexBufferObjectHandle { get; }
        int VertexArrayObjectHandle { get; }

        void IDisposable.Dispose()
        {
            GL.DeleteVertexArray(VertexArrayObjectHandle);
            GL.DeleteBuffer(VertexBufferObjectHandle);
        }

        int IHandle.Handle => VertexArrayObjectHandle;

        void IBindable.Bind()
        {
            GL.BindVertexArray(Handle);
        }

        void IRenderable.Render()
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, Count);
        }

        void Render(PrimitiveType primitiveType)
        {
            GL.DrawArrays(primitiveType, 0, Count);
        }

        void Render(int index, int count)
        {
            GL.DrawArrays(PrimitiveType.Triangles, index, count);
        }

        void Render(int index, int count, PrimitiveType primitiveType)
        {
            GL.DrawArrays(primitiveType, index, count);
        }
    }
}