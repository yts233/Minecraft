using OpenTK.Mathematics;
using System;

namespace Minecraft.Input
{
    internal class PointerAxisInput : IPointerAxisInput
    {
        private readonly IPointerContainer _container;
        private float _sensibility;

        public PointerAxisInput(IPointerContainer container)
        {
            _container = container;
        }
        public AxisRange Range => AxisRange.Full;

        public float Sensibility
        {
            get => _sensibility;
            set
            {
                if (value > 1.0F || value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value should belongs to [0.0F, 1.0F]");
                _sensibility = value;
            }
        }
        public Vector3 Value { get; private set; }
        public bool ZeroOnInactivate { get; set; }

        public void Update()
        {
            var state = _container.PointerState;
            if (_sensibility < 0.00000001F)
            {
                var position = state.Position * 2.0F;
                Value = ZeroOnInactivate && !_container.PointerActivated
                    ? Vector3.Zero
                    : new Vector3(position.X / _container.ClientSize.X - 1.0F, -position.Y / _container.ClientSize.Y + 1.0F, 0.0F);
                return;
            }
            var delta = state.Delta * _sensibility;
            Value = new Vector3(delta.X, -delta.Y, 0.0F);
        }
    }
}