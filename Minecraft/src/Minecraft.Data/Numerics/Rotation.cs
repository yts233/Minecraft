using System.Collections.Generic;

namespace Minecraft.Data.Numerics
{
    public struct Rotation : IVector2<float>
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        float IVector2<float>.X { get => Yaw; set => Yaw = value; }
        float IVector2<float>.Y { get => Pitch; set => Pitch = value; }

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
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"({Yaw}F, {Pitch}F)";
        }
    }
}