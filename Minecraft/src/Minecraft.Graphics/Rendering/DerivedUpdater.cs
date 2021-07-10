using System.Diagnostics.CodeAnalysis;

namespace Minecraft.Graphics.Rendering
{
    public class DerivedUpdater : IUpdatable
    {
        [MaybeNull] public IUpdatable BaseUpdater { get; set; }

        public void Update()
        {
            BaseUpdater?.Update();
        }
    }
}