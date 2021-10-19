using System;
using System.Collections.Generic;

namespace Minecraft.Numerics
{
    public struct Vector3i : IVector3<int>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int LengthPow2 => X * X + Y * Y + Z * Z;

        public int Length => (int)Math.Sqrt(LengthPow2);

        int IList<int>.this[int index]
        {
            get => ((IVector3<int>)this)[index];
            set => ((IVector3<int>)this)[index] = value;
        }

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public override string ToString()
        {
            return $"({X}D, {Y}D, {Z}D)";
        }

        public static implicit operator Vector3i((int x, int y, int z) value)
        {
            return new Vector3i { X = value.x, Y = value.y, Z = value.z };
        }


        /// <summary>
        /// Add the value
        /// </summary>
        /// <param name="other"></param>
        /// <remarks>The operation is like:<code>current = current + other</code></remarks>
        public void Add(IVector3<int> other)
        {
            X += other.X;
            Y += other.Y;
            Z += other.Z;
        }

        /// <summary>
        /// Delta the value
        /// </summary>
        /// <param name="other"></param>
        /// <remarks>The operation is like:<code>current = current - other</code></remarks>
        public void Delta(IVector3<int> other)
        {
            X -= other.X;
            Y -= other.Y;
            Z -= other.Z;
        }

        /// <summary>
        /// Scale the value
        /// </summary>
        /// <param name="other"></param>
        /// <remarks>The operation is like:<code>current = current * other</code></remarks>
        public void Scale(int other)
        {
            X *= other;
            Y *= other;
            Z *= other;
        }

        public void Normalize()
        {
            var len = Length;
            X /= len;
            Y /= len;
            Z /= len;
        }

    }
}