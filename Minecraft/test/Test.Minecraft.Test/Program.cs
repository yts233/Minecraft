using System;
using Minecraft;
using static System.Console;

namespace Test.Minecraft.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var id = new NamedIdentifier("yts233:test_script.js");
            WriteLine(id);
            WriteLine(id.Namespace);
            WriteLine(id.Name);
            WriteLine(id.IsValid);
        }
    }
}