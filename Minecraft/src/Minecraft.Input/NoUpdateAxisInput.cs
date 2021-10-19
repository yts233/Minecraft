using OpenTK.Mathematics;

namespace Minecraft.Input
{
    internal class NoUpdateAxisInput : IExternAxisInput
    {
        private readonly IAxisInput _input;

        public NoUpdateAxisInput(IAxisInput input)
        {
            _input = input;
        }

        public AxisRange Range => _input.Range;

        public Vector3 Value => _input.Value;

        public IAxisInput BaseInput => _input;

        public void Update()
        {
        }
    }
}