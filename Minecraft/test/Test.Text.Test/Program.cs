using System;
using System.IO;
using Minecraft.Text;

namespace Test.Text.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var stream = new MemoryStream();
            stream.WriteByte(100);
            stream.WriteByte(100);
            stream.WriteByte(100);
            stream.WriteByte(100);
            stream.Position = 0;

            var reader = new Utf8Reader(stream);
            Console.WriteLine(reader.ReadToEnd());
            Console.WriteLine(stream.Position);
            Console.WriteLine(stream.Length);
        }
    }
}