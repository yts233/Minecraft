using System;
using System.Collections.Generic;
using System.Linq;
using Minecraft;
using Minecraft.Data;
using Minecraft.Data.Vanilla;

namespace Test.Data.Blocks.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger.SetExceptionHandler();
            Logger.SetThreadName("MainThread");
            Logger.GetLogger<Program>().HelloWorld("World");
            var chuck = new EmptyChunk();
            var rand = new Random();
            for (var i = 0; i < 10; i++)
                chuck.SetBlock(rand.Next(15), rand.Next(256), rand.Next(15), VanillaBlockIds.DiamondBlock);
            foreach (var (x, y, z, block) in chuck.EnumerateBlocks().Where(b => b.block.Equals(VanillaBlockIds.DiamondBlock)))
                Logger.GetLogger<Program>().Info($"{(x, y, z)}: {block.Name}");

            Logger.GetLogger<Program>().Info($"Testing collections");

            Logger.WaitForLogging();

            {
                var a = new Queue<byte>();
                var t1 = DateTime.Now;
                for (var i = 0; i < 10485760; i++) //10 MB
                    a.Enqueue(1);
                Logger.GetLogger<Program>().Info($"Queue: {(DateTime.Now - t1).TotalMilliseconds}ms");
            } //testing Queue

            Logger.WaitForLogging();

            {
                var a = new Stack<byte>();
                var t1 = DateTime.Now;
                for (var i = 0; i < 10485760; i++) //10 MB
                    a.Push(1);
                Logger.GetLogger<Program>().Info($"Stack: {(DateTime.Now - t1).TotalMilliseconds}ms");
            } // testing Stack

            Logger.WaitForLogging();

            {
                var a = new List<byte>();
                var t1 = DateTime.Now;
                for (var i = 0; i < 10485760; i++) //10 MB
                    a.Add(1);
                Logger.GetLogger<Program>().Info($"List: {(DateTime.Now - t1).TotalMilliseconds}ms");
            } // testing List

            Logger.WaitForLogging();

            {
                var a = new byte[10485760];
                var t1 = DateTime.Now;
                for (var i = 0; i < 10485760; i++) //10 MB
                    a[i] = 0;
                Logger.GetLogger<Program>().Info($"Array: {(DateTime.Now - t1).TotalMilliseconds}ms");
            } // testing Array



            Logger.WaitForLogging();
        }
    }
}