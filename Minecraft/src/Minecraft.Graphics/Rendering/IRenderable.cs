namespace Minecraft.Graphics.Rendering
{
    public interface IRenderable
    {
        /// <summary>
        /// ��Ⱦ�˶���
        /// </summary>
        /// <remarks>����˶���̳�<see cref="IBindable"/>������ִ��<see cref="IBindable.Bind"/></remarks>
        void Render();
    }
}