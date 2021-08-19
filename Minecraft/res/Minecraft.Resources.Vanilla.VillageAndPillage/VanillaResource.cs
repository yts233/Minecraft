using System;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Minecraft.Resources.Vanilla.VillageAndPillage
{
    public class VanillaResource : ResourceDirectory
    {
        private static readonly System.Resources.ResourceManager ResourceManager;

        static VanillaResource()
        {
            ResourceManager = new System.Resources.ResourceManager("Minecraft.Resources.Vanilla.VillageAndPillage.Resources",
                Assembly.GetExecutingAssembly());
        }

        public VanillaResource() : base(
            new ComplexFilePath(new IFilePath[]
            {
                new HashFilePath(MinecraftPaths.AssetsObjects, MinecraftPaths.AssetsIndexes["1.14.json"]),
                new ZipFilePath(ResourceManager.GetStream("Vanilla"))
            }),
            "Minecraft Village & Pillage Resource Pack")
        {
        }
    }
}