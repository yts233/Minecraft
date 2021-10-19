using Minecraft;
using Minecraft.Data;
using Minecraft.Data.Common.Chunking;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Blocking;
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
        private IElementArrayHandle _triangle;
        private SimpleShader _shader;
        private VanillaResource _resource;
        private readonly IChunk _chunk;
        private readonly IEye _eye = new Eye();
        //private readonly CameraMotivatorRenderer _cameraMotivatorRenderer;
        private readonly IBlockEditor _chunkEditor;
        private readonly IViewTransformProvider _viewTransform;
        private readonly IPerspectiveTransformProvider _projectionTransform;
        private readonly ChunkRenderer _chunkRenderer;
        private TextureAtlas _atlases;

        public MainWindow()
        {
            PointerGrabbed = true;
            var chunk = new EmptyChunk();
            _chunk = chunk;
            _chunkEditor = chunk;
            _chunkEditor.Fill(0, 0, 0, 15, 0, 15, "cyan_wool");
            _viewTransform = _eye.GetViewTransformProvider();
            _projectionTransform = _eye.GetPerspectiveTransformProvider();
            _chunkRenderer = new ChunkRenderer(_chunk, () => _atlases, _viewTransform, _projectionTransform);
            var cameraMotivatorRenderer = new CameraMotivatorRenderer(_eye);
            cameraMotivatorRenderer.Controlable = true;
            var keyInput = this.CreateKeyAxisInput();
            var pointerInput = this.CreatePointerAxisInput();
            //input.IsOctagon = true;
            keyInput.PositiveXKey = Keys.D;
            keyInput.NegativeXKey = Keys.A;
            keyInput.PositiveZKey = Keys.S;
            keyInput.NegativeZKey = Keys.W;
            keyInput.PositiveYKey = Keys.Space;
            keyInput.NegativeYKey = Keys.LeftShift;
            pointerInput.ZeroOnInactivate = true;
            pointerInput.Sensibility = 0.1F;
            cameraMotivatorRenderer.PositionInput = keyInput.GetSmoothAxisInput();
            cameraMotivatorRenderer.RotationInput = pointerInput.GetSmoothAxisInput(speed: 0.8F);
            this.AddObject(new ViewportRenderer(this));
            this.AddUpdater(() => _eye.Aspect = ClientSize.Y == 0 ? 1.0F : (float)ClientSize.X / ClientSize.Y);
            this.AddUpdater(cameraMotivatorRenderer);
            this.AddInitializer(_chunkRenderer);
            this.AddRenderer(_chunkRenderer);
        }

        protected override void OnBeforeInitalizers(object sender, EventArgs e)
        {
            GL.ClearColor(Color4.CornflowerBlue);

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

            _logger.Info("Finished building texture atlases.");

            _triangle = new ElementArray(new VertexArray<float>(new[] {
                -.5F,-.5F, 1F,0F,0F,
                .5F,-.5F, 0F,1F,0F,
                .5F,.5F, 0F,0F,1F,
                -.5F,.5F, 0F,1F,0F
            }, new VertexAttributePointer[] {
                new()
                {
                    Index=0,
                    Normalized=false,
                    Offset=0,
                    Size=2,
                    Type=VertexAttribePointerType.Float
                },
                new ()
                {
                    Index=1,
                    Normalized=false,
                    Offset=2*sizeof(float),
                    Size=3,
                    Type=VertexAttribePointerType.Float
                }
            }).GetHandle(), new uint[] {
                0,1,2,
                0,2,3
            }).GetHandle();

            _shader = new SimpleShader();

            base.OnBeforeInitalizers(sender, e);
        }

        protected override void OnBeforeRenderers(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            _shader.View = _viewTransform.GetMatrix();
            _shader.Projection = _projectionTransform.GetMatrix();

            _triangle.Bind();
            _triangle.Render();

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
