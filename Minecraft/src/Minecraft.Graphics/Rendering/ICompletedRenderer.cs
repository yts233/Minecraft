using System;

namespace Minecraft.Graphics.Rendering
{
    public interface ICompletedRenderer : IInitializer, IRenderable, IUpdatable, IBindable, ITickable, IDisposable
    {
    }
}