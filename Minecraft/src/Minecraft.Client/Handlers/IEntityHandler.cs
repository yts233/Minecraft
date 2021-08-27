using Minecraft.Numerics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minecraft.Client.Handlers
{
    public interface IEntityHandler
    {
        int EntityId { get; }
        Uuid EntityUuid { get; }
        Vector3d Position { get; }
        Rotation Rotation { get; }
        bool OnGround { get; }
        bool IsValid { get; internal set; }
        IPositionHandler GetPositionHandler();
    }
    public interface IVelocityEntityHandler : IEntityHandler
    {
        Vector3d Velocity { get; }
    }
    public interface IBlockBasedPositionEntityHandler : IEntityHandler
    {
        Vector3d IEntityHandler.Position => (Vector3d)Position;
        new Vector3i Position { get; }
    }
    public interface IBlockBasedRotationEntityHandler : IEntityHandler
    {
        Rotation IEntityHandler.Rotation => Rotation.FromDirection(Direction);
        Direction Direction { get; }
    }
    public interface IMobEntityHandler : IVelocityEntityHandler
    {
        int Type { get; }
    }
    public interface INonLivingEntityHandler : IMobEntityHandler
    {
        int Data { get; }
    }
    public interface ILivingEntityHandler : IMobEntityHandler
    {
        float HeadPitch { get; }
    }
    public interface IPlayerHandler : IEntityHandler
    {
    }
}
