using Minecraft.Input;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Windowing;
using Minecraft.Graphics.Transforming;
using Minecraft.Graphics.Renderers.Environments.Clouding;
using Minecraft.Graphics.Renderers.Utils;
using Minecraft.Resources.Vanilla.VillageAndPillage;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using Minecraft.Graphics.Texturing;
using Minecraft;
using System.Linq;
using Minecraft.Resources;
using Minecraft.Data;
using Minecraft.Graphics.Renderers.Blocking;

namespace Demo.MCGraphicsCloud
{
    public class MainWindow : RenderWindow
    {
        private readonly VanillaResource _resource;
        private readonly IEye _eye;
        private readonly IViewTransformProvider _viewTransformProvider;
        private readonly IPerspectiveTransformProvider _projectionTransformProvider;
        private readonly CameraMotivatorRenderer _cameraMotivatorRenderer;
        private readonly IAxisInput _eyeInput;
        private readonly IAxisInput _moveInput;
        private readonly EmptyWorld _world;
        private ITexture2DAtlas _atlases;

        public MainWindow()
        {
            _resource = new VanillaResource();
            _eye = new Eye
            {
                DepthFar = 512F,
                Position = (0, 15, 256)
            };
            _viewTransformProvider = _eye.GetViewTransformProvider();
            _projectionTransformProvider = _eye.GetPerspectiveTransformProvider();
            _eyeInput = this.CreatePointerAxisInput(.5F, true).CreateSmoothAxisInput();
            _moveInput = this.CreateKeyAxisInput(Keys.D, Keys.Space, Keys.S, Keys.A, Keys.LeftShift, Keys.W, true).CreateSmoothAxisInput();
            _world = new EmptyWorld
            {
                ChunkProvider = (x, z) => new EmptyChunk { X = x, Z = z, World = _world }
            };
            _world.FillFast(0, 0, 0, 256, 0, 256, "bedrock");
            _world.FillFast(0, 1, 0, 256, 10, 256, "iron_block");
            _world.FillFast(0, 11, 0, 256, 11, 256, "diamond_block");

            _cameraMotivatorRenderer = new CameraMotivatorRenderer(_eye)
            {
                RotationInput = _eyeInput,
                PositionInput = _moveInput,
                MovementSpeed = 1F,
                Type = CameraType.Fps,
                Controlable = true
            };
            this.AddUpdater(_cameraMotivatorRenderer);
            this.AddCompletedRenderer(new CloudRenderer(_eye, _viewTransformProvider, _projectionTransformProvider, _resource));
            this.AddCompletedRenderer(new WorldRenderer(_world, () => _atlases, _viewTransformProvider, _projectionTransformProvider) { Camera = _eye, AutoSetCenterChunk = true });
            PointerGrabbed = true;
        }

        protected override void OnBeforeInitalizers(object sender, EventArgs e)
        {
            var assets = _resource.GetAssets().Where(asset =>
                        asset.Type == AssetType.Texture &&
                        asset.NamedIdentifier.Name.StartsWith("block/") &&
                        asset.NamedIdentifier.Name.EndsWith(".png"));
            var textureBuilder = new TextureAtlasBuilder();
            var i = 0;
            foreach (var asset in assets)
            {
                using var stream = asset.OpenRead();
                Logger.GetLogger<MainWindow>().Info(asset.NamedIdentifier.FullName);
                var bImg = new Image(stream);
                var isSingle = bImg.FrameCount == 1;
                var q = 0;
                foreach (var image in bImg)
                {
                    textureBuilder.Add(isSingle ? asset.NamedIdentifier.FullName : $"{asset.NamedIdentifier.FullName}{{{q}}}", image);
                    i++;
                    q++;
                    if (i == 4096)
                        break;
                }

                if (i == 4096)
                    break;
            }

            _atlases = textureBuilder.Build();


            GL.ClearColor(Color4.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            base.OnBeforeInitalizers(sender, e);
        }

        protected override void OnBeforeRenderers(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            _viewTransformProvider.Calculate();
            _projectionTransformProvider.Calculate();
            base.OnBeforeRenderers(sender, e);
        }

        protected override void OnBeforeUpdaters(object sender, EventArgs e)
        {
            _eyeInput.Update();
            _moveInput.Update();

            /*_eye.Position += _moveInput.Value * .005F;*/
            /*_eye.Rotation += _eyeInput.Value.Xy * .0025F;*/
            base.OnBeforeUpdaters(sender, e);
        }

        protected override void OnRenderClientSizeChanged(object sender, Vector2i e)
        {
            GL.Viewport(0, 0, e.X, e.Y);
            _eye.Aspect = e.X / Math.Max(e.Y, .001F);
            base.OnRenderClientSizeChanged(sender, e);
        }
    }
}
