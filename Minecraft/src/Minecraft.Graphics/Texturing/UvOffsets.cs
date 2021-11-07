namespace Minecraft.Graphics.Texturing
{
    /// <summary>
    /// TextureCoord Offsets
    /// </summary>
    /// <remarks>in bytes</remarks>
    public readonly struct UvOffsets
    {
        public readonly int ButtonLeft;
        public readonly int ButtonRight;
        public readonly int TopLeft;
        public readonly int TopRight;
 
        public UvOffsets(int buttonLeft, int buttonRight, int topLeft, int topRight)
        {
            ButtonLeft = buttonLeft;
            ButtonRight = buttonRight;
            TopLeft = topLeft;
            TopRight = topRight;
        }
    }
}