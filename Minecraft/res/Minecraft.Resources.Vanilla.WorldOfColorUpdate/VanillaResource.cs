using System.Reflection;
using System.Resources;

namespace Minecraft.Resources.Vanilla.WorldOfColorUpdate
{
    public class VanillaResource : ResourceDirectory
    {
        private static readonly ResourceManager ResourceManager;

        static VanillaResource()
        {
            ResourceManager = new ResourceManager("Minecraft.Resources.Vanilla.WorldOfColorUpdate.Resources",
                Assembly.GetExecutingAssembly());
        }

        public VanillaResource() : base(
            new ComplexFilePath(new IFilePath[]
            {
                new HashFilePath(MinecraftPaths.AssetsObjects, MinecraftPaths.AssetsIndexes["1.12.json"]),
                new ZipFilePath(ResourceManager.GetStream("Vanilla"))
            }),
            "Minecraft World of Color Update Resource Pack")
        {
        }
    }
}