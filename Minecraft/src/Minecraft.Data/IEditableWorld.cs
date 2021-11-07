namespace Minecraft.Data
{
    public interface IEditableWorld : IWorld, IBlockEditor
    {
        bool AddChunk(IChunk chunk);
        bool RemoveChunk(int x, int z);
    }
}
