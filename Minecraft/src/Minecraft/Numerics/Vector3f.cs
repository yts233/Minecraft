using System;
using System.Collections.Generic;

namespace Minecraft.Numerics
{
    public struct Vector3f : IVector3<float>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float LengthSquared => X * X + Y * Y + Z * Z;

        public float Length => (float)Math.Sqrt(LengthSquared);

        float IList<float>.this[int index]
        {
            get => ((IVector3<float>)this)[index];
            set => ((IVector3<float>)this)[index] = value;
        }

        public void Deconstruct(out float x, out float y, out float z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public override string ToString()
        {
            return $"({X}D, {Y}D, {Z}D)";
        }

        /// <summary>
        /// Add the value
        /// </summary>
        /// <param name="other"></param>
        /// <remarks>The operation is like:<code>current = current + other</code></remarks>
        public void Add(IVector3<float> other)
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
        public void Delta(IVector3<float> other)
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
        public void Scale(float other)
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

        public static implicit operator Vector3f((float x, float y, float z) value)
        {
            return new Vector3f { X = value.x, Y = value.y, Z = value.z };
        }

        public static explicit operator Vector3f(Vector3i value)
        {
            return new Vector3f { X = value.X, Y = value.Y, Z = value.Z };
        }
    }
}