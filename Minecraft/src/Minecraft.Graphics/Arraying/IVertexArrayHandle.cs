using System;
using Minecraft.Graphics.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace Minecraft.Graphics.Arraying
{
    public interface IVertexArrayHandle : IHandle, IBindable, IRenderable, IDisposable
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
            Render(0, Count);
        }

        void Render(int index, int count)
        {
            GL.DrawArrays(PrimitiveType.Triangles, index, count);
        }
    }
}