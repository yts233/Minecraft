using System;
using System.Threading.Tasks;
using Minecraft;
using Minecraft.Resources;

namespace Test.Resources.FilePaths.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Info<Program>("Hello World!");
            IFilePath core =
                new ZipFilePath(new FilePath("/home/ye_tianshun/.minecraft/versions/1.16.4/core.zip"));
            IFilePath root = new ComplexFilePath(new[]
            {
                new HashFilePath(new FilePath("/home/ye_tianshun/.minecraft/assets/objects"),
                    new FilePath("/home/ye_tianshun/.minecraft/assets/indexes/1.16.json")),
                core
            });
            foreach (var filePath in root["minecraft"]["textures"])
            {
                Logger.Debug<Program>(filePath.PathName);
            }


            foreach (var filePath in ((IFilePath) new FilePath(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))[
                ".minecraft"])
            {
                Logger.Debug<Program>(filePath.PathName);
            }
        }
    }
}