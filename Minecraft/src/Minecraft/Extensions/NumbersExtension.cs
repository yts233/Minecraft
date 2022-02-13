using OpenTK.Mathematics;
using System;

namespace Minecraft.Extensions
{
    public static class MathExtension
    {
        public static int GetBitsCount(this int i)
        {
            var count = 0;
            while (i != 0)
            {
                count++;
                i >>= 1;
            }

            return count;
        }

        public static Vector2 ToRotation(this Direction direction)
        {
            return direction switch
            {
                Direction.North => new Vector2 { X = 0 },
                Direction.West => new Vector2 { X = 90 },
                Direction.South => new Vector2 { X = 180 },
                Direction.East => new Vector2 { X = 270 },
                _ => throw new ArgumentOutOfRangeException(nameof(direction)),
            };
        }

        private const double Epsilon = 0D;

        public static double ClipXCollide(this Box2d b, Box2d c, double xa)
        {
            if (c.Max.Y <= b.Min.Y || c.Min.Y >= b.Max.Y)
                return xa;
            if (xa > 0.0D && c.Max.X <= b.Min.X)
            {
                double max = b.Min.X - c.Max.X - Epsilon;
                if (max < xa)
                    xa = max;
            }
            if (xa < 0.0D && c.Min.X >= b.Max.X)
            {
                double max = b.Max.X - c.Min.X + Epsilon;
                if (max > xa)
                    xa = max;
            }
            return xa;
        }
        
        public static double ClipYCollide(this Box2d b, Box2d c, double ya)
        {
            if (c.Max.X <= b.Min.X || c.Min.X >= b.Max.X)
                return ya;
            if (ya > 0.0D && c.Max.Y <= b.Min.Y)
            {
                double max = b.Min.Y - c.Max.Y - Epsilon;
                if (max < ya)
                    ya = max;
            }
            if (ya < 0.0D && c.Min.Y >= b.Max.Y)
            {
                double max = b.Max.Y - c.Min.Y + Epsilon;
                if (max > ya)
                    ya = max;
            }
            return ya;
        }
    }
}