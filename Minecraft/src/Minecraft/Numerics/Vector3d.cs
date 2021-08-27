using System;
using System.Collections.Generic;

namespace Minecraft.Numerics
{
    public struct Vector3d : IVector3<double>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double LengthPow2 => Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2);

        public double Length => Math.Sqrt(LengthPow2);

        double IList<double>.this[int index]
        {
            get => ((IVector3<double>)this)[index];
            set => ((IVector3<double>)this)[index] = value;
        }

        public void Deconstruct(out double x, out double y, out double z)
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
        public void Add(IVector3<double> other)
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
        public void Delta(IVector3<double> other)
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
        public void Scale(double other)
        {
            X *= other;
            Y *= other;
            Z *= other;
        }

        public static implicit operator Vector3d((double x, double y, double z) value)
        {
            return new Vector3d { X = value.x, Y = value.y, Z = value.z };
        }

        public static explicit operator Vector3d(Vector3i value)
        {
            return new Vector3d { X = value.X, Y = value.Y, Z = value.Z };
        }
    }
}