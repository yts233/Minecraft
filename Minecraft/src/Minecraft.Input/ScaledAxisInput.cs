namespace Minecraft.Input
{
    internal class ScaledAxisInput : IScaledAxisInput
    {
        public float Scale { get; set; } = 1F;

        public IAxisInput BaseInput { get; }

        public AxisRange Range => BaseInput.Range;

        public ScaledAxisInput(IAxisInput input)
        {
            BaseInput = input;
        }

        public void Update()
        {
            BaseInput.Update();
        }
    }
}