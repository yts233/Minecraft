using System;

namespace Minecraft.Graphics.Rendering
{
    public interface ICompletedRenderer : IInitializer, IRenderable, IUpdatable, ITickable, IDisposable
    {
    }
}