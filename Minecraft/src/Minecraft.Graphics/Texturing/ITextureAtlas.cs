using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Texturing
{
    public interface ITexture2DAtlas : ITexture2D, IReadOnlyDictionary<NamedIdentifier, Box2>
    {
    }
}