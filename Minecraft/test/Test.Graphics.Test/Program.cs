using System;
using Minecraft.Graphics.Transforming;

namespace Test.Graphics.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var transformProvider = new ModelTransformProvider
            {
                Model = new Model
                {
                    Translation = (1, 0, 0),
                    Rotation = (90, 90, 90),
                    Scale = (2, 2, 2)
                }
            };
            transformProvider.CalculateMatrix();
            Console.WriteLine(transformProvider.Transform((0F, 1F, 0F, 1F)));
        }
    }
}