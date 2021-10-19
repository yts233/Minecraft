namespace Minecraft.Input
{
    public interface IKeyAxisInput : IAxisInput
    {
        Keys? PositiveXKey { get; set; }
        Keys? NegativeXKey { get; set; }
        Keys? PositiveYKey { get; set; }
        Keys? NegativeYKey { get; set; }
        Keys? PositiveZKey { get; set; }
        Keys? NegativeZKey { get; set; }
        bool IsOctagon { get; set; }
    }
}