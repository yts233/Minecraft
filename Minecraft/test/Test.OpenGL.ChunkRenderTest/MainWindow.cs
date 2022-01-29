using Minecraft;
using Minecraft.Data;
using Minecraft.Data.Vanilla;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Renderers.Blocking;
using Minecraft.Graphics.Renderers.Utils;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Transforming;
using Minecraft.Graphics.Windowing;
using Minecraft.Input;
using Minecraft.Resources;
using Minecraft.Resources.Vanilla.VillageAndPillage;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Linq;

namespace Test.OpenGL.ChunkRenderTest
{
    class MainWindow : RenderWindow
    {
        private static Logger<MainWindow> _logger = Logger.GetLogger<MainWindow>();
        private SimpleShader _shader;
        private VanillaResource _resource;
        private readonly IEye _eye = new Eye();
        //private readonly CameraMotivatorRenderer _cameraMotivatorRenderer;
        private readonly IEditableWorld _world;
        private readonly IViewTransformProvider _viewTransform;
        private readonly IPerspectiveTransformProvider _projectionTransform;
        private readonly WorldRenderer _worldRenderer;
        private readonly PerformanceWatcherRenderer _performanceWatcherRenderer = new();
        private ITexture2DAtlas _textureAtlas;

        public MainWindow()
        {
            PointerGrabbed = false;
            _world = new EmptyWorld();
            _world.AddChunk(new EmptyChunk() { X = 0, Z = 0, World = _world });
            _world.AddChunk(new EmptyChunk() { X = 0, Z = 1, World = _world });
            _world.AddChunk(new EmptyChunk() { X = 1, Z = 0, World = _world });
            _world.Fill(0, 0, 0, 31, 0, 31, VanillaBlocks.Bedrock);
            _world.Fill(0, 1, 0, 31, 10, 31, VanillaBlocks.Dirt);
            _world.Fill(0, 11, 0, 31, 11, 31, VanillaBlocks.DiamondBlock);
            _viewTransform = _eye.GetViewTransformProvider();
            _projectionTransform = _eye.GetPerspectiveTransformProvider();
            _worldRenderer = new WorldRenderer(_world, () => _textureAtlas, _viewTransform, _projectionTransform)
            {
                ViewDistance = 8,
                CachedDistance = 8
            };
            var cameraMotivatorRenderer = new CameraMotivatorRenderer(_eye)
            {
                Controlable = true,
                Speed = .25F
            };
            //var pointerInput = this.CreatePointerAxisInput(
            //    zeroOnInactivate: false,
            //    sensibility: 0.1F);
            cameraMotivatorRenderer.PositionInput = this.CreateKeyAxisInput(
                positionXKey: Keys.D,
                positionYKey: Keys.Space,
                positionZKey: Keys.S,
                negativeXKey: Keys.A,
                negativeYKey: Keys.LeftShift,
                negativeZKey: Keys.W).CreateSmoothAxisInput();
            _eye.Position = (0F, 0F, 5F);
            cameraMotivatorRenderer.RotationInput = this.CreateKeyAxisInput(
                positionXKey: Keys.Right,
                negativeXKey: Keys.Left,
                negativeYKey: Keys.Up,
                positionYKey: Keys.Down)
                .CreateSmoothAxisInput();
            //cameraMotivatorRenderer.RotationInput = pointerInput.GetSmoothAxisInput(speed: 0.8F);
            this.AddRenderObject(new ViewportRenderer(this));
            this.AddRenderObject(_performanceWatcherRenderer);
            this.AddTicker(_performanceWatcherRenderer);
            this.AddUpdater(() => _eye.Aspect = ClientSize.Y == 0 ? 1.0F : (float)ClientSize.X / ClientSize.Y);
            this.AddUpdater(cameraMotivatorRenderer);
            this.AddRenderObject(_worldRenderer);
            this.AddIntervalTicker(20, () => Title = $"ChunkRenderTest - FPS: {_performanceWatcherRenderer.LastRenderTimes} ({_performanceWatcherRenderer.LastUpdateTimes})");

            //this.AddIntervalTicker(100, _chunkRenderer.Update); // update the chunk
        }

        protected override void OnBeforeInitalizers(object sender, EventArgs e)
        {
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);

            //load assets
            _resource = new VanillaResource();

            var assets = _resource.GetAssets().Where(asset =>
                        asset.Type == AssetType.Texture &&
                        asset.NamedIdentifier.Name.StartsWith("block/") &&
                        asset.NamedIdentifier.Name.EndsWith(".png"));
            var textureBuilder = new TextureAtlasBuilder();
            var i = 0;
            foreach (var asset in assets)
            {
                using var stream = asset.OpenRead();
                //Logger.GetLogger<Program>().Info(asset.NamedIdentifier.FullName);
                var bImg = new Image(stream);
                var isSingle = bImg.FrameCount == 1;
                var q = 0;
                foreach (var image in bImg)
                {
                    textureBuilder.Add(isSingle ? asset.NamedIdentifier : new NamedIdentifier(asset.NamedIdentifier.Namespace, $"{asset.NamedIdentifier.Name}{{{q}}}"), image);
                    i++;
                    q++;
                    if (i == 4096)
                        break;
                }

                if (i == 4096)
                    break;
            }

            _textureAtlas = textureBuilder.Build();

            _logger.Info("Finished building texture atlases.");

            _shader = new SimpleShader();

            base.OnBeforeInitalizers(sender, e);
        }

        protected override void OnBeforeRenderers(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            _shader.View = _viewTransform.GetMatrix();
            _shader.Projection = _projectionTransform.GetMatrix();

            base.OnBeforeRenderers(sender, e);
        }

        protected override void OnBeforeUpdaters(object sender, EventArgs e)
        {
            _viewTransform.Calculate();
            _projectionTransform.Calculate();
            //Console.WriteLine(_eye.Rotation);
            base.OnBeforeUpdaters(sender, e);
        }

    }
}
