using System;

namespace Minecraft.Resources
{
    public static class MinecraftPaths
    {
        public static IFilePath MinecraftRoot { get; set; }
        public static IFilePath AssetsRoot { get; set; }
        public static IFilePath AssetsIndexes { get; set; }
        public static IFilePath AssetsObjects { get; set; }

        static MinecraftPaths()
        {
            AutoLoadPaths(Environment.OSVersion.Platform switch
            {
                PlatformID.Unix => ((IFilePath) new FilePath(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)))[".minecraft"],
                PlatformID.Win32NT => ((IFilePath) new FilePath(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))[".minecraft"],
                PlatformID.MacOSX => ((IFilePath) new FilePath(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)))["Library"][
                        "Application Support"]
                    ["minecraft"],
                _ => null
            });
        }

        public static void AutoLoadPaths(IFilePath minecraftRoot)
        {
            MinecraftRoot = minecraftRoot;
            AssetsRoot = minecraftRoot?["assets"];
            AssetsIndexes = AssetsRoot?["indexes"];
            AssetsObjects = AssetsRoot?["objects"];
        }
    }
}