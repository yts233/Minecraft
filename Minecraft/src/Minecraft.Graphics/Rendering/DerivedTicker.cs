using System.Diagnostics.CodeAnalysis;

namespace Minecraft.Graphics.Rendering
{
    public class DerivedTicker : ITickable
    {
        [MaybeNull] public ITickable BaseTicker { get; set; }

        public void Tick()
        {
            BaseTicker?.Tick();
        }
    }
}