using System;
using System.IO;
using System.Text;

namespace Tool.PacketRedirector
{
    public class StreamPrinter
    {
        public StreamPrinter(Stream baseStream)
        {
            BaseStream = baseStream;
        }

        public Stream BaseStream { get; }

        public void Print()
        {
            var buffer = new byte[8192];
            int length;
            while ((length = BaseStream.Read(buffer, 0, 8192)) != 0)
            {
                var buff = new MemoryStream(buffer, 0, length, false);
                var @string = new StringBuilder();
                @string.Append('\n');
                @string.Append(DateTime.Now.ToString("H:mm:ss"));
                @string.Append("\tlength:" + length);
                @string.Append('\n');
                @string.Append("Address  00 11 22 33 44 55 66 77 88 99 AA BB CC DD EE FF  0123456789ABCDEF\n");
                while (buff.Position < buff.Length)
                {
                    var sb = new StringBuilder();
                    @string.Append("0x" + buff.Position.ToString("X").PadLeft(5, '0') + "  ");
                    for (var i = 0; i < 16; i++)
                    {
                        var tmp = buff.ReadByte();
                        if (tmp == -1)
                        {
                            sb.Append(' ');
                            @string.Append(".. ");
                            continue;
                        }

                        sb.Append(tmp >= 32 ? (char)tmp : '.');
                        @string.Append(tmp.ToString("X").PadLeft(2, '0') + " ");
                    }

                    @string.Append(' ');
                    @string.Append(sb);
                    @string.Append('\n');
                }

                Console.WriteLine(@string);
            }
        }
    }
}