using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Protocol.Client.Handlers
{
    public interface IEntityHandler : IValidHandler
    {
        int EntityId { get; }
        Uuid EntityUuid { get; }
        Vector3d Position { get; }
        Vector2 Rotation { get; }
        bool OnGround { get; }
        IPositionHandler GetPositionHandler();
    }
}
