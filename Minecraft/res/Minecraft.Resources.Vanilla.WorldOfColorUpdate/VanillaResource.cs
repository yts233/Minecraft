using System.Reflection;

namespace Minecraft.Resources.Vanilla.WorldOfColorUpdate
{
    public class VanillaResource : DirectoryResource
    {
        private static readonly System.Resources.ResourceManager ResourceManager;

        static VanillaResource()
        {
            ResourceManager = new System.Resources.ResourceManager("Minecraft.Resources.Vanilla.WorldOfColorUpdate.Resources",
                Assembly.GetExecutingAssembly());
        }

        public VanillaResource() : base(
            new FilePathMap(new IFilePath[]
            {
                new HashFilePath(MinecraftPaths.AssetsObjects, MinecraftPaths.AssetsIndexes["1.12.json"]),
                new ZipFilePath(ResourceManager.GetStream("Vanilla"))
            }),
            "Minecraft World of Color Update Resource Pack")
        {
        }
    }
}