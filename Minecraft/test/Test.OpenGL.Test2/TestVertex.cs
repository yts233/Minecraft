using System.Runtime.InteropServices;

namespace Test.OpenGL.Test2
{
    [StructLayout(LayoutKind.Sequential)]
    struct TestVertex
    {
        public float X;
        public float Y;
        public float CX;
        public float CY;
        public float CZ;
    }
}