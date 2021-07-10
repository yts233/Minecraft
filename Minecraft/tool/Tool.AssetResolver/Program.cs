using System;
using System.Linq;

namespace Tool.AssetResolver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var filename = args.Length == 0 ? Console.ReadLine() : args[0];
            var infos = AssetFileInfo.Resolve(filename).ToList();
            foreach (var info in infos)
                Console.WriteLine($@"
Hash: {info.Hash}
Name: {info.Name}
Size: {info.Size}");
        }
    }
}