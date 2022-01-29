using Minecraft;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Renderers.UI;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Windowing;
using Minecraft.Input;
using Minecraft.Resources;
using Minecraft.Resources.Fonts;
using Minecraft.Resources.Vanilla.VillageAndPillage;
//using Minecraft.Resources.Vanilla.WorldOfColorUpdate;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Linq;

var resourceManager = new ResourceManager(() =>
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

resourceManager.Reload();

Font font = new(resourceManager, "default", forceUnicodeFont: false);

TestShader shader = null;
//ITexture texture = null;
ITexture2DAtlas texture = null;
//ASCIIVertexProvider avp = new() { Color = Color4.Aqua, Offset = (0F, 0F), Value = "Good Night" };
//IElementArrayHandle eah = null;

Vector3 position = Vector3.Zero;
IAxisInput input = null;

var renderWindow = new RenderWindow();

HudRenderer hud = new(renderWindow, () => texture, font);
TextHudObject tho = new() { Text = "你好，世界！ Hello World!", FontScale = (32F, 32F) };
hud.Add(tho);
renderWindow.AddRenderObject(hud);

void OnLoad()
{
    input = renderWindow.CreateKeyAxisInput(Keys.Right, Keys.Down, null, Keys.Left, Keys.Up, null, false).CreateSmoothAxisInput();
    //input = renderWindow.CreatePointerAxisInput(.005F, true).CreateSmoothAxisInput();
    shader = new TestShader();
    var textureBuilder = new TextureAtlasBuilder();
    foreach (var asset in resourceManager.GetAssets().Where(p => p.Type == AssetType.Texture && p.NamedIdentifier.Name.StartsWith("font")))
    {
        Logger.GetLogger<Program>().Info($"Load Texture: {asset.NamedIdentifier}.");
        textureBuilder.Add(asset.NamedIdentifier, new Image(asset.OpenRead(), true));
    }
    texture = textureBuilder.Build();

    GL.ClearColor(Color4.Black);
}

void OnRender()
{
    GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
}

void OnUpdate()
{
    input.Update();
    position -= input.Value * .001F;
}

void OnResize(Vector2i size)
{
    GL.Viewport(0, 0, size.X, size.Y);
    shader.Use();
    //shader.View = Matrix4.LookAt((size.X / 2, 0, size.Y / 2), (0, 0, -1), (0, 1, 0));
    shader.Projection = Matrix4.CreateOrthographicOffCenter(0F, size.X, size.Y, 0, -10F, 100F);
    tho.MultiLineWidth = size.X;
}


renderWindow.BeforeInitalizers += (sender, e) => OnLoad();
renderWindow.BeforeRenderers += (sender, e) => OnRender();
renderWindow.BeforeUpdaters += (sender, e) => OnUpdate();
renderWindow.RenderClientSizeChanged += (sender, e) => OnResize(e);
renderWindow.Run();

