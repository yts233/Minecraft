using Minecraft.Graphics.Arraying;
using System.Collections.Generic;

namespace Test.OpenGL.Test2
{
    class TestVertexProvider : IVertexArrayProvider<TestVertex>
    {
        public IEnumerable<uint> GetIndices()
        {
            yield return 0;
            yield return 1;
            yield return 2;
            yield return 0;
            yield return 2;
            yield return 3;
        }

        public IEnumerable<VertexAttributePointer> GetPointers()
        {
            yield return new VertexAttributePointer
            {
                Index = 0,
                Normalized = false,
                Offset = 0,
                Size = 2,
                Type = VertexAttribePointerType.Float
            };
            yield return new VertexAttributePointer
            {
                Index = 1,
                Normalized = false,
                Offset = 2 * sizeof(float),
                Size = 3,
                Type = VertexAttribePointerType.Float
            };
        }

        public IEnumerable<TestVertex> GetVertices()
        {
            yield return new TestVertex
            {
                X = -1F,
                Y = -1F,
                CX = 1F,
                CY = 0F,
                CZ = 0F
            };
            yield return new TestVertex
            {
                X = 1F,
                Y = -1F,
                CX = 1F,
                CY = 1F,
                CZ = 0F
            };
            yield return new TestVertex
            {
                X = 1F,
                Y = 1F,
                CX = 1F,
                CY = 0F,
                CZ = 1F
            };

            yield return new TestVertex
            {
                X = -1F,
                Y = 1F,
                CX = 0F,
                CY = 1F,
                CZ = 1F
            };
        }
    }
}