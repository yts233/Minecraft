using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Demo.MCGraphics2D
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Col2dVertex
    {
        public Vector2 Position;
        public Vector4 Color;
    }
}
