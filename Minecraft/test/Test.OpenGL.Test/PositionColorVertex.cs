using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Test.OpenGL.Test
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexObject
    {
        public VertexObject(Vector3 position, Vector3 color, Vector2 textureCoord)
        {
            Position = position;
            Color = color;
            TextureCoord = textureCoord;
        }

        public Vector3 Position { get; set; }
        public Vector3 Color { get; set; }
        public Vector2 TextureCoord { get; set; }

        public void Deconstruct(out Vector3 position, out Vector3 color, out Vector2 textureCoord)
        {
            position = Position;
            color = Color;
            textureCoord = TextureCoord;
        }

        public static implicit operator VertexObject((Vector3 position, Vector3 color, Vector2 textureCoord) values)
        {
            return new VertexObject(values.position, values.color, values.textureCoord);
        }
    }
}