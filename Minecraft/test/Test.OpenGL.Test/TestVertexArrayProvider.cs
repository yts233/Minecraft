using System.Collections.Generic;
using Minecraft.Graphics.Arraying;

namespace Test.OpenGL.Test
{
    public class TestVertexArrayProvider : IVertexArrayProvider<VertexObject>
    {

        public IEnumerable<VertexAttributePointer> GetPointers()
        {
            yield return new VertexAttributePointer // pos
            {
                Index = 0,
                Normalized = false,
                Type = VertexAttribePointerType.Float,
                Offset = 0,
                Size = 3
            };
            yield return new VertexAttributePointer //color
            {
                Index = 1,
                Normalized = false,
                Type = VertexAttribePointerType.Float,
                Offset = 3 * sizeof(float),
                Size = 3
            };
            yield return new VertexAttributePointer //textureCoord
            {
                Index = 2,
                Normalized = false,
                Type = VertexAttribePointerType.Float,
                Offset = 6 * sizeof(float),
                Size = 2
            };
        }
        
        public IEnumerable<uint> GetIndices()
        {
            yield return 0;
            yield return 1;
            yield return 3;
            yield return 0;
            yield return 3;
            yield return 2;
        }

        private int _a = 0;
        
        public IEnumerable<VertexObject> GetVertices()
        {
            yield return ((-.5F, -.5F, _a), (1F, 1F, 1F), (0F, 0F));
            yield return ((.5F, -.5F, _a), (1F, 1F, 1F), (1F, 0F));
            yield return ((-.5F, .5F, _a), (1F, 1F, 1F), (0F, 1F));
            yield return ((.5F, .5F, _a), (1F, 1F, 1F), (1F, 1F));
            _a++;
        }
    }
}