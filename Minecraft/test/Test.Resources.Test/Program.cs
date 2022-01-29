using System;
using System.IO;
using System.Linq;
using Minecraft;
using Minecraft.Extensions;
using Minecraft.Resources.Fonts;
using Minecraft.Resources.Vanilla.VillageAndPillage;

namespace Test.Resources.Test
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Loading resource and font...");
            using var resource = new VanillaResource();
            Logger.WaitForLogging();

            Console.WriteLine("Finished.");

            var font = new Font(resource, "default");

            while (true)
            {
                var line = Console.ReadLine();
                foreach (var c in line)
                {
                    (var file, var x1, var y1, var x2, var y2) = font.GetChar(c).Value;
                    Console.WriteLine($"{c}\t\\u{(int)c:x4}\t{file}\t{x1}\t{x2}\t{y1}\t{y2}");
                }
                Console.WriteLine("--------------------------------");
            }

            //Console.ReadKey();
        }
    }
}