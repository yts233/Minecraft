using System.Collections.Generic;

namespace Minecraft.Resources.Fonts
{
    public class UnicodeFontProvider : IFontProvider
    {
        private readonly byte[] _size = new byte[65536];

        public IReadOnlyList<byte> Sizes => _size;

        public string Template { get; }

        public int Row => 16;

        public int Column => 16;

        public UnicodeFontProvider(Asset sizes, string template)
        {
            sizes.OpenRead().Read(_size, 0, 65536);
            Template = template;
        }

        public (NamedIdentifier file, float x1, float y1, float x2, float y2)? GetChar(char c)
        {
            if (c > '\uffff')
                return null;
            NamedIdentifier file = Template.Replace("%s", (c >> 8).ToString("x2"));
            int x = (c & 0b1111) << 4;
            int y = c & 0b11110000;
            float unit = 1F / 256;
            return (file, unit * (x + (_size[c] >> 4)), unit * y, unit * (x + (_size[c] & 0b1111) + 1), unit * (y + 16));
        }
    }
}
