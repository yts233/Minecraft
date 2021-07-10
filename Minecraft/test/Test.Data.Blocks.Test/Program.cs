using System;
using System.Linq;
using Minecraft;
using Minecraft.Data.Common.Chunking;
using Minecraft.Data.Vanilla;

namespace Test.Data.Blocks.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger.SetExceptionHandler();
            Logger.SetThreadName("MainThread");
            Logger.Info<Program>("Hello World!");
            var chuck = new ChuckData();
            var rand = new Random();
            for (var i = 0; i < 10; i++)
                chuck.SetBlock(VanillaBlockIds.DiamondBlock, rand.Next(15), rand.Next(256), rand.Next(15));
            foreach (var (position, block) in chuck.GetBlocks().Where(b => b.blockState == VanillaBlockIds.DiamondBlock)
            )
                Logger.Info<Program>($"{position.ToBlockPositionInChuck()}: {block.Name}");
        }
    }
}