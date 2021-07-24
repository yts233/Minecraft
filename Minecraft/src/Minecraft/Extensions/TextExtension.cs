using System.IO;
using System.Text;

namespace Minecraft.Extensions
{
    public static class TextExtension
    {
        public static string ToEscape(this string s)
        {
            var reader = new StringReader(s);
            var builder = new StringBuilder();
            int read;
            while ((read = reader.Read()) != -1)
            {
                var @char = (char)read;

                builder.Append(@char switch
                {
                    '\a' => "\\a",
                    '\b' => "\\b",
                    '\n' => "\\n",
                    '\r' => "\\r",
                    '\t' => "\\t",
                    '\v' => "\\v",
                    '\0' => "\\0",
                    '\"' => "\\\"",
                    '\'' => "\\'",
                    '\\' => "\\\\",
                    _ => @char.ToString()
                });
            }

            return builder.ToString();
        }
    }
}