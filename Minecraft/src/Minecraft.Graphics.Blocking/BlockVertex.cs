﻿using System.Runtime.InteropServices;

namespace Minecraft.Graphics.Blocking
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BlockVertex
    {
        public float X;
        public float Y;
        public float Z;
        public float U;
        public float V;
        public float NX;
        public float NY;
        public float NZ;
        public BlockVertex(float x, float y, float z, float u, float v, float nx, float ny, float nz)
        {
            X = x;
            Y = y;
            Z = z;
            U = u;
            V = v;
            NX = nx;
            NY = ny;
            NZ = nz;
        }
    }
}