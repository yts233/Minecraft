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
            yield return new VertexAttributePointer
            {
                Index = 1,
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
                /* Position // Normal */
                // bottom 0 - 3
                0, 0, 0, /**/ 0, -1, 0,
                1, 0, 0, /**/ 0, -1, 0,
                1, 0, 1, /**/ 0, -1, 0,
                0, 0, 1, /**/ 0, -1, 0,
                // top 4 - 7
                0, 1, 0, /**/ 0, 1, 0,
                1, 1, 0, /**/ 0, 1, 0,
                1, 1, 1, /**/ 0, 1, 0,
                0, 1, 1, /**/ 0, 1, 0,
                // left 8 - 11
                0, 0, 0, /**/ -1, 0, 0,
                0, 0, 1, /**/ -1, 0, 0,
                0, 1, 1, /**/ -1, 0, 0,
                0, 1, 0, /**/ -1, 0, 0,
                // right 12 - 15
                1, 0, 0, /**/ 1, 0, 0,
                1, 0, 1, /**/ 1, 0, 0,
                1, 1, 1, /**/ 1, 0, 0,
                1, 1, 0, /**/ 1, 0, 0,
                // front 16 - 19
                0, 0, 0, /**/0, 0, -1,
                1, 0, 0, /**/0, 0, -1,
                1, 1, 0, /**/0, 0, -1,
                0, 1, 0, /**/0, 0, -1,
                // back 20 - 23
                0, 0, 1, /**/0, 0, 1,
                1, 0, 1, /**/0, 0, 1,
                1, 1, 1, /**/0, 0, 1,
                0, 1, 1, /**/0, 0, 1
            };
        }

        public IEnumerable<uint> GetIndices()
        {
            return new uint[]
            {
                // bottom 0 - 5
                0, 1, 2,
                0, 2, 3,
                // top 6 - 11
                6, 5, 4,
                7, 6, 4,
                // left 12 - 17
                8, 9, 10,
                8, 10, 11,
                // right 18 - 23
                14, 13, 12,
                15, 14, 12,
                // front 24 - 29
                19, 17, 16,
                19, 18, 17,
                // back 30 - 36
                20, 21, 23,
                21, 22, 23,
            };
        }
    }
}