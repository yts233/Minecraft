using System.Collections.Generic;

namespace Minecraft.Graphics.Arraying
{
    public interface IVertexArrayProvider<out T> where T : struct
    {
        IEnumerable<VertexAttributePointer> GetPointers();

        IEnumerable<T> GetVertices();

        IEnumerable<uint> GetIndices();
    }
}