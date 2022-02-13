using System.Collections.Generic;
using System.Linq;

namespace Minecraft.Resources.Fonts
{
    public class BitmapFontProvider : IFontProvider
    {
        private readonly string[] _chars;
        public NamedIdentifier File { get; }
        public int Ascent { get; }
        public int Height { get; }
        public IReadOnlyList<string> Chars => _chars;
        public int Column { get; }
        public int Row { get; }

        public BitmapFontProvider(NamedIdentifier file, int ascent, IEnumerable<string> chars, int height = 16)
        {
            File = file;
            Ascent = ascent;
            _chars = chars.ToArray();
            Column = Chars[0].Length;
            Row = Chars.Count;
            if (!Chars.All(p => p.Length == Column))
                throw new ResourceException("Failed to load bitmap provider.");
            Height = height;
        }

        public (NamedIdentifier file, float x1, float y1, float x2, float y2)? GetChar(char c)
        {
            for (int i = 0; i < _chars.Length; i++)
            {
                for (int j = 0; j < _chars[i].Length; j++)
                {
                    if (_chars[i][j] == c)
                    {
                        float unit = 1F / Column;
                        return (File, unit * j, unit * i, unit * (j + 1), unit * (i + 1));
                    }
                }
            }
            return null;
        }
    }
}
