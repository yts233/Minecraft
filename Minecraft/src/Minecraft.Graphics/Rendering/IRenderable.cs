namespace Minecraft.Graphics.Rendering
{
    public interface IRenderable
    {
        /// <summary>
        /// 渲染此对象
        /// </summary>
        /// <remarks>如果此对象继承<see cref="IBindable"/>，请先执行<see cref="IBindable.Bind"/></remarks>
        void Render();
    }
}