using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Minecraft.Graphics.Renderers.Worlds.Blocking
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CommonBlockVertex
    {
        public CommonBlockVertex(Vector3 position, Vector3 color, Vector3 normal, Vector2 textureCoord)
        {
            Position = position;
            Color = color;
            Normal = normal;
            TextureCoord = textureCoord;
        }

        public Vector3 Position { get; set; }
        public Vector3 Color { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 TextureCoord { get; set; }

        public void Deconstruct(out Vector3 position, out Vector3 color, out Vector3 normal, out Vector2 textureCoord)
        {
            position = Position;
            color = Color;
            normal = Normal;
            textureCoord = TextureCoord;
        }

        public static implicit operator CommonBlockVertex((Vector3 position,Vector3 color,Vector3 normal,Vector2 textureCoord) values)
        {
            return new CommonBlockVertex(values.position, values.color, values.normal, values.textureCoord);
        }
    }
}
