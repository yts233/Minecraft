using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Demo.MCGraphics2D
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Tex2dVertex
    {
        public Vector2 Position;
        public Vector2 TexCoord;
    }
}
