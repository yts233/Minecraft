namespace Minecraft.Resources.Fonts
{
    public interface IFontProvider
    {
        int Row { get; }
        int Column { get; }

        (NamedIdentifier file, float x1, float y1, float x2, float y2)? GetChar(char c);
    }
}
