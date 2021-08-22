using System.Diagnostics.CodeAnalysis;

namespace Minecraft.Graphics.Rendering
{
    public class DerivedInitializer : IInitializer
    {
        [MaybeNull] public IInitializer BaseInitializer { get; set; }

        public void Initialize()
        {
            BaseInitializer?.Initialize();
        }
    }
}