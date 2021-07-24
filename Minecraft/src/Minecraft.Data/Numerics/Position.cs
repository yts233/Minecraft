using System.Collections.Generic;

namespace Minecraft.Data.Numerics
{
    public struct Vector3d : IVector3<double>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

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
    }
}