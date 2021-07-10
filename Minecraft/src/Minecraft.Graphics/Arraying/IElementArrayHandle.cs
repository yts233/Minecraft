using System;
using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace Minecraft.Graphics.Arraying
{
    public interface IElementArrayHandle : IVertexArrayHandle
    {
        new int Count { get; }
        int ElementBufferObjectHandle { get; }

        void IDisposable.Dispose()
        {
            GL.DeleteBuffer(ElementBufferObjectHandle);
        }

        int IHandle.Handle => VertexArrayObjectHandle;

        void IBindable.Bind()
        {
            GL.BindVertexArray(Handle);
        }

        void IRenderable.Render()
        {
            GL.DrawElements(PrimitiveType.Triangles, Count, DrawElementsType.UnsignedInt, 0);
        }

        new void Render(int index, int count)
        {
            GL.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, index * sizeof(uint));
        }

        void DisposeAll()
        {
            GL.DeleteVertexArray(VertexArrayObjectHandle);
            GL.DeleteBuffer(VertexBufferObjectHandle);
            GL.DeleteBuffer(ElementBufferObjectHandle);
        }
    }
}