using System.Collections.Generic;
using Minecraft.Graphics.Arraying;

namespace Minecraft.Graphics.Renderers.Environments.Clouding
{
    public class CloudVertexArrayProvider : IVertexArrayProvider<float>
    {
        public IEnumerable<VertexAttributePointer> GetPointers()
        {
            yield return new VertexAttributePointer
            {
                Index = 0,
                Normalized = false,
                Offset = 0,
                Size = 3,
                Type = VertexAttribePointerType.Float
            };
        }

        public IEnumerable<float> GetVertices()
        {
            return new[]
            {
                0F, 0F, 0F,
                12F, 0F, 0F,
                12F, 1F, 0F,
                0F, 1F, 0F,
                0F, 0F, 12F,
                12F, 0F, 12F,
                12F, 1F, 12F,
                0F, 1F, 12F
            };
        }

        public IEnumerable<uint> GetIndices()
        {
            return new uint[]
            {
                // back
                0, 3, 1,
                1, 3, 2,

                // front
                4, 5, 6,
                4, 6, 7,

                // left
                0, 4, 7,
                0, 7, 3,

                // right
                1, 2, 5,
                2, 6, 5,

                // bottom
                0, 1, 5,
                0, 5, 4,

                // top
                3, 7, 6,
                3, 6, 2
            };
        }
    }
}