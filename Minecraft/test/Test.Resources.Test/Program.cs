using System;
using Minecraft.Resources.Vanilla.VillageAndPillage;

namespace Test.Resources.Test
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using var resource = new VanillaResource();
            foreach (var asset in resource.GetAssets())
                Console.WriteLine(asset);
            Console.ReadKey();
        }
    }
}