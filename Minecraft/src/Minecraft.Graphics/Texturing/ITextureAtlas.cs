using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Texturing
{
    public interface ITextureAtlas : ITexture, IReadOnlyDictionary<NamedIdentifier, Box2>
    {
    }
}