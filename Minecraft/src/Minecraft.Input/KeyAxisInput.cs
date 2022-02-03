using OpenTK.Mathematics;
using System;

namespace Minecraft.Input
{
    internal class KeyAxisInput : IKeyAxisInput
    {
        private readonly IKeyboardContainer _keyboardContainer;
        private bool _isOctagon;

        public KeyAxisInput(IKeyboardContainer keyboardContainer)
        {
            _keyboardContainer = keyboardContainer;
        }

        public AxisRange Range { get; private set; } = AxisRange.Full;
        public bool IsOctagon
        {
            get => _isOctagon;
            set
            {
                Range = value ? AxisRange.Octagon : AxisRange.Full;
                _isOctagon = value;
            }
        }

        public Vector3 Value { get; private set; }

        public Keys? PositiveXKey { get; set; }
        public Keys? NegativeXKey { get; set; }
        public Keys? PositiveYKey { get; set; }
        public Keys? NegativeYKey { get; set; }
        public Keys? PositiveZKey { get; set; }
        public Keys? NegativeZKey { get; set; }

        public void Update()
        {
            bool px, py, pz, nx, ny, nz;
            px = PositiveXKey.HasValue && _keyboardContainer.KeyboardState[PositiveXKey.Value];
            py = PositiveYKey.HasValue && _keyboardContainer.KeyboardState[PositiveYKey.Value];
            pz = PositiveZKey.HasValue && _keyboardContainer.KeyboardState[PositiveZKey.Value];
            nx = NegativeXKey.HasValue && _keyboardContainer.KeyboardState[NegativeXKey.Value];
            ny = NegativeYKey.HasValue && _keyboardContainer.KeyboardState[NegativeYKey.Value];
            nz = NegativeZKey.HasValue && _keyboardContainer.KeyboardState[NegativeZKey.Value];
            var value = new Vector3(Convert.ToInt32(px) - Convert.ToInt32(nx), Convert.ToInt32(py) - Convert.ToInt32(ny), Convert.ToInt32(pz) - Convert.ToInt32(nz));
            if (_isOctagon && value.LengthSquared > 0.0000001F)
                Value = value.Normalized();
            else Value = value;
        }
    }
}