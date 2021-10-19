using Minecraft.Numerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Client.Handlers
{
    public interface IEntityHandler : IValidHandler
    {
        int EntityId { get; }
        Uuid EntityUuid { get; }
        Vector3d Position { get; }
        Rotation Rotation { get; }
        bool OnGround { get; }
        IPositionHandler GetPositionHandler();
    }
}
