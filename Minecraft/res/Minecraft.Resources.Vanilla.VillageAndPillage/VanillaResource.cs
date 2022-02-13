using System;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Minecraft.Resources.Vanilla.VillageAndPillage
{
    public class VanillaResource : DirectoryResource
    {
        private static readonly System.Resources.ResourceManager ResourceManager;

        static VanillaResource()
        {
            ResourceManager = new System.Resources.ResourceManager("Minecraft.Resources.Vanilla.VillageAndPillage.Resources",
                Assembly.GetExecutingAssembly());
        }

        public VanillaResource() : base(
            GetFilePath(),
            "Minecraft Village & Pillage Resource Pack")
        {
        }

        public static IFilePath GetFilePath()
        {
            return new FilePathMap(new IFilePath[]
            {
                new HashFilePath(MinecraftPaths.AssetsObjects, MinecraftPaths.AssetsIndexes["1.14.json"]),
                new ZipFilePath(ResourceManager.GetStream("Vanilla"))
            });
        }
    }
}