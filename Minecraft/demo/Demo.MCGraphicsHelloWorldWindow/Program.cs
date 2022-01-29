using Minecraft;
using Minecraft.Graphics.Renderers.UI;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Shading;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Windowing;
using Minecraft.Resources;
using Minecraft.Resources.Fonts;
using Minecraft.Resources.Vanilla.VillageAndPillage;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Linq;

namespace Demo.MCGraphicsHelloWorldWindow
{
    public class MainWindow : RenderWindow
    {
        private readonly HudRenderer _hudRenderer;
        private readonly ResourceManager _resourceManager;
        private readonly Font _font;
        private readonly TextHudObject _tho = new() { Color = Color4.White, Text = "Hello World", FontScale = (64F, 64F) };
        private ITexture2DAtlas _texture = null;
        public MainWindow()
        {
            Title = "Hello World";
            _resourceManager = new ResourceManager(() =>
                new Resource[] {
                    new VanillaResource()
                }, res =>
                {
                    foreach (var r in res)
                    {
                        r.Dispose();
                    }
                }
            );
            _resourceManager.Reload();
            _font = new Font(_resourceManager, "default");
            _hudRenderer = new(this, () => _texture, _font);
            _hudRenderer.Add(_tho);
            this.AddRenderObject(_hudRenderer);
            _font = new(_resourceManager, "default", forceUnicodeFont: false);
        }

        protected override void OnBeforeInitalizers(object sender, EventArgs e)
        {
            var textureBuilder = new TextureAtlasBuilder();
            foreach (var asset in _resourceManager.GetAssets().Where(p => p.Type == AssetType.Texture && p.NamedIdentifier.Name.StartsWith("font")))
            {
                //Logger.GetLogger<Program>().Info($"Load Texture: {asset.NamedIdentifier}.");
                textureBuilder.Add(asset.NamedIdentifier, new Image(asset.OpenRead(), true));
            }
            _texture = textureBuilder.Build();

            base.OnBeforeInitalizers(sender, e);
        }

        protected override void OnAfterInitalizers(object sender, EventArgs e)
        {
            ThreadHelper.StartThread(() =>
            {
                while (true)
                {
                    Console.Write("\nInput something to display: ");
                    _tho.Text = Console.ReadLine();
                }
            }, isBackground: true);

            base.OnAfterInitalizers(sender, e);
        }

        protected override void OnBeforeRenderers(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            base.OnBeforeRenderers(sender, e);
        }

        protected override void OnRenderClientSizeChanged(object sender, Vector2i e)
        {
            GL.Viewport(0, 0, e.X, e.Y);
            base.OnRenderClientSizeChanged(sender, e);
        }
        protected override void OnClientSizeChanged(object sender, Vector2i e)
        {
            _tho.MultiLineWidth = e.X;
            base.OnClientSizeChanged(sender, e);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            RenderWindow.InvokeOnGlfwThread(() => new MainWindow()).Run();
        }
    }
}
