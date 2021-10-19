using OpenTK.Mathematics;
using System;

namespace Minecraft.Input
{
    internal class SmoothAxisInput : ISmoothAxisInput
    {
        private readonly IAxisInput _input;

        public SmoothAxisInput(IAxisInput input)
        {
            _input = input;
        }

        private float _speed = 0.125F;
        public float Speed
        {
            get => _speed;
            set
            {
                if (value > 1.0F || value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value should belongs to (0.0F, 1.0F]");
                _speed = value;
            }
        }

        public AxisRange Range => _input.Range;

        public Vector3 Value { get; private set; }

        public IAxisInput BaseInput => _input;

        private bool _first = true;

        public void Update()
        {
            _input.Update();
            var value = _input.Value;
            if (_first)
            {
                Value = value;
                _first = false;
            }
            var delta = _input.Value - Value;
            if (delta.LengthSquared < 0.000001F)
            {
                Value = value;
                return;
            }
            Value += delta * Speed;
        }
    }
}