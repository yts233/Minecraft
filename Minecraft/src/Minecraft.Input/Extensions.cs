namespace Minecraft.Input
{
    public static class Extensions
    {
        public static ISmoothAxisInput GetSmoothAxisInput(this IAxisInput input, float speed = 0.125F)
        {
            return new SmoothAxisInput(input)
            {
                Speed = speed
            };
        }

        public static IKeyAxisInput CreateKeyAxisInput(this IKeyboardContainer container,
            Keys? positionXKey = null,
            Keys? positionYKey = null,
            Keys? positionZKey = null,
            Keys? negativeXKey = null,
            Keys? negativeYKey = null,
            Keys? negativeZKey = null,
            bool isOctagon = false)
        {
            return new KeyAxisInput(container)
            {
                PositiveXKey = positionXKey,
                PositiveYKey = positionYKey,
                PositiveZKey = positionZKey,
                NegativeXKey = negativeXKey,
                NegativeYKey = negativeYKey,
                NegativeZKey = negativeZKey,
                IsOctagon = isOctagon
            };
        }

        public static IPointerAxisInput CreatePointerAxisInput(this IPointerContainer container,
            float sensibility = 0.0F,
            bool zeroOnInactivate = false)
        {
            return new PointerAxisInput(container)
            {
                Sensibility = sensibility,
                ZeroOnInactivate = zeroOnInactivate
            };
        }

        public static IExternAxisInput LockUpdate(this IAxisInput input)
        {
            return new NoUpdateAxisInput(input);
        }
    }
}