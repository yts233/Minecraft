using System.Collections.Generic;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Rendering;

namespace Minecraft.Graphics.Renderers.Debuggers.Axis
{
    public class AxisVertexProvider : IVertexArrayProvider<float>
    {
        public IEnumerable<VertexAttributePointer> GetPointers()
        {
            yield return new VertexAttributePointer
            {
                Index = 0,
                Normalized = true,
                Offset = 0,
                Size = 3,
                Type = VertexAttribePointerType.Float
            };
            yield return new VertexAttributePointer
            {
                Index = 0,
                Normalized = true,
                Offset = 3 * sizeof(float),
                Size = 3,
                Type = VertexAttribePointerType.Float
            };
        }

        public IEnumerable<float> GetVertices()
        {
            return new float[]
            {
                0, 0, 0, 1, 0, 0,
                1, 0, 0, 1, 0, 0,
                0, 0, 0, 0, 1, 0,
                0, 1, 0, 0, 1, 0,
                0, 0, 0, 0, 0, 1,
                0, 0, 1, 0, 0, 1
            };
        }

        public IEnumerable<uint> GetIndices()
        {
            return new uint[] {0, 1, 2, 3, 4, 5, 6};
        }
    }
}