using System;
using System.Collections.Generic;

namespace Minecraft.Numerics
{
    public struct Rotation : IVector2<float>
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        float IVector2<float>.X { get => Yaw; set => Yaw = value; }
        float IVector2<float>.Y { get => Pitch; set => Pitch = value; }

        public float LengthPow2 => Yaw*Yaw+Pitch*Pitch;

        public float Length =>(float)Math.Sqrt(LengthPow2);

        float IList<float>.this[int index]
        {
            get => ((IVector2<float>)this)[index];
            set => ((IVector2<float>)this)[index] = value;
        }

        public void Deconstruct(out float yaw, out float pitch)
        {
            yaw = Yaw;
            pitch = Pitch;
        }

        void IVector2<float>.Deconstruct(out float x, out float y)
        {
            x = Yaw;
            y = Pitch;
        }

        public override string ToString()
        {
            return $"({Yaw}F, {Pitch}F)";
        }

        public static implicit operator Rotation((float yaw, float pitch) value)
        {
            return new Rotation { Yaw = value.yaw, Pitch = value.pitch };
        }

        public void Normalize()
        {
            var yaw = Yaw;
            var pitch = Pitch;
            yaw %= 180F;
            Yaw = yaw < 0 ? yaw + 180F : yaw;
            Pitch = pitch > 89.9F
                ? 89.9F
                : pitch < -89.9F
                    ? -89.9F
                    : pitch;
        }

        public static Rotation FromDirection(Direction direction)
        {
            return direction switch
            {
                Direction.North => new Rotation { Yaw = 0 },
                Direction.West => new Rotation { Yaw = 90 },
                Direction.South => new Rotation { Yaw = 180 },
                Direction.East => new Rotation { Yaw = 270 },
                _ => throw new ArgumentOutOfRangeException(nameof(direction)),
            };
        }

        /// <summary>
        /// Add the value
        /// </summary>
        /// <param name="other"></param>
        /// <remarks>The operation is like:<code>current = current + other</code></remarks>
        public void Add(IVector2<float> other)
        {
            Yaw += other.X;
            Pitch += other.Y;
        }

        /// <summary>
        /// Delta the value
        /// </summary>
        /// <param name="other"></param>
        /// <remarks>The operation is like:<code>current = current - other</code></remarks>
        public void Delta(IVector2<float> other)
        {
            Yaw -= other.X;
            Pitch -= other.Y;
        }

        /// <summary>
        /// Scale the value
        /// </summary>
        /// <param name="other"></param>
        /// <remarks>The operation is like:<code>current = current * other</code></remarks>
        public void Scale(float other)
        {
            Yaw *= other;
            Pitch *= other;
        }

    }
}