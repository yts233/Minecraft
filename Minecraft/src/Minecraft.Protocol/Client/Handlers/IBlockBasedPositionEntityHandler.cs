﻿using OpenTK.Mathematics;

namespace Minecraft.Protocol.Client.Handlers
{
    public interface IBlockBasedPositionEntityHandler : IEntityHandler
    {
        Vector3d IEntityHandler.Position => Position;
        new Vector3i Position { get; }
    }
}
