namespace Minecraft.Graphics.Rendering
{
    public class BindRenderInvoker : IBindable, IRenderable
    {

        public BindRenderInvoker(IBindable binder, IRenderable renderer)
        {
            Binder = binder;
            Renderer = renderer;
        }

        public BindRenderInvoker()
        {
        }

        public IBindable Binder { get; set; }
        public IRenderable Renderer { get; set; }

        void IBindable.Bind()
        {
            Binder?.Bind();
        }

        public void Render()
        {
            Binder?.Bind();
            Renderer?.Render();
        }
    }
}