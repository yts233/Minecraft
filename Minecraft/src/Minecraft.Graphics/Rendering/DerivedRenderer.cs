using System.Diagnostics.CodeAnalysis;

namespace Minecraft.Graphics.Rendering
{
    public class DerivedRenderer : IRenderable
    {
        [MaybeNull] public IRenderable BaseRenderer { get; set; }

        public void Render()
        {
            BaseRenderer?.Render();
        }
    }
}