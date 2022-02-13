using System.Reflection;

namespace Minecraft.Resources.Vanilla.MC117Resource
{
    public class VanillaResource : DirectoryResource
    {
        private static readonly System.Resources.ResourceManager ResourceManager;

        static VanillaResource()
        {
            ResourceManager = new System.Resources.ResourceManager("Minecraft.Resources.Vanilla.MC117Resource.Resources",
                Assembly.GetExecutingAssembly());
        }

        public VanillaResource() : base(
            GetFilePath(),
            "Minecraft 1.17 Resource Pack")
        {
        }

        public static IFilePath GetFilePath()
        {
            return new FilePathMap(new IFilePath[]
            {
                new HashFilePath(MinecraftPaths.AssetsObjects, MinecraftPaths.AssetsIndexes["1.17.json"]),
                new ZipFilePath(ResourceManager.GetStream("Vanilla"))
            });
        }
    }
}
