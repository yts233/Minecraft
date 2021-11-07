namespace Minecraft.Data
{
    public interface IEditableChunk : IChunk, IBlockEditor
    {
        new int X { get; set; }
        new int Z { get; set; }
        new IWorld World { get; set; }
        int IChunk.X => X;
        int IChunk.Z => Z;
        IWorld IChunk.World => World;
    }
}
