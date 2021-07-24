using Minecraft.Protocol.Data;
using Minecraft.Text;
using System;
using System.IO;

namespace Test.Protocol.Data.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var stream = new MemoryStream();
            var content = new ByteArray(stream);
            content.Write(123L);
            content.Position = 0;
            var value = content.ReadLong();
            Console.WriteLine(value);
        }
    }
}