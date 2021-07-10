using System;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Tool.ResourceGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            switch (args.Length)
            {
                case 4:
                {
                    var method = args[3] switch
                    {
                        "import" => 1,
                        "export" => 2,
                        _ => throw new ArgumentException("{4}", nameof(args))
                    };

                    switch (method)
                    {
                        case 1:
                        {
                            using var writer = new ResourceWriter(args[0]);
                            using var stream = File.OpenRead(args[2]);
                            writer.AddResource(args[1], stream);
                            writer.Generate();
                            writer.Close();
                            break;
                        }
                        case 2:
                        {
                            using var reader = new ResourceReader(args[0]);
                            using var stream = File.OpenWrite(args[2]);
                            reader.GetResourceData(args[1], out var resourceType, out var resourceData);
                            stream.Write(resourceData, 0, resourceData.Length);
                            reader.Close();
                            break;
                        }
                    }

                    break;
                }
                case 5:
                {
                    var assembly = Assembly.LoadFrom(args[0]);
                    var rm = new ResourceManager(args[1], assembly);
                    using var stream1 = rm.GetStream(args[2]);
                    using var stream2 = File.OpenWrite(args[3]);
                    stream1.CopyTo(stream2);
                    stream1.Close();
                    stream2.Close();
                    rm.ReleaseAllResources();
                    break;
                }
                // case 2:
                // {
                //     using var writer = new ResourceWriter(args[0]);
                //
                //     writer.Generate();
                //
                //     writer.Close();
                //     break;
                // }
                default:
                    Console.WriteLine(
                        "arguments1:\n{0}: resource file path\n{1}: item name\n{2}: file path\n{3}= import\n");
                    Console.WriteLine(
                        "arguments2:\n{0}: resource file path\n{1}: item name\n{2}: file path\n{3}= export\n");
                    Console.WriteLine(
                        "arguments2:\n{0}: assembly file path\n{1}: base name\n{2}: item name\n{3}: file path\n{4}= export\n");
                    return;
            }
        }
    }
}