using Minecraft;
using Minecraft.Extensions;
using Minecraft.Input;
using Minecraft.Client;
using Minecraft.Data;
using Minecraft.Graphics.Renderers.Utils;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Transforming;
using Minecraft.Graphics.Windowing;
using System;
using OpenTK.Mathematics;
using Minecraft.Graphics.Blocking;
using Minecraft.Graphics.Texturing;
using Minecraft.Resources;
using OpenTK.Graphics.OpenGL4;
using Minecraft.Resources.Vanilla.VillageAndPillage;
using System.Linq;
using Minecraft.Protocol.Data;
using System.Threading.Tasks;

namespace Test.MinecraftClientAndOpenGL.Test
{
    class MainWindow : RenderWindow
    {
        private static Logger<MainWindow> _logger = Logger.GetLogger<MainWindow>();

        private readonly IEye _camera;
        private readonly IViewTransformProvider _viewMatrixProvider;
        private readonly IPerspectiveTransformProvider _perspectiveTransformProvider;
        private readonly IEditableWorld _world;
        private readonly WorldRenderer _worldRenderer;
        private readonly MinecraftClient _client;
        private readonly IThreadDispatcher _clientThread;
        private readonly string _serverAddress;
        private readonly ushort _serverPort;
        private ITextureAtlas _atlases;
        private VanillaResource _resource;

        public MainWindow(string serverAddress = "localhost", ushort serverPort = 25565)
        {
            _camera = new Eye();
            _viewMatrixProvider = _camera.GetViewTransformProvider();
            _perspectiveTransformProvider = _camera.GetPerspectiveTransformProvider();
            _world = new EmptyWorld();
            _worldRenderer = new WorldRenderer(_world, () => _atlases, _viewMatrixProvider, _perspectiveTransformProvider)
            {
                CachedDistance = 8,
                ViewDistance = 4
            };
            _clientThread = ThreadHelper.NewThread("ClientThread");
            _client = new MinecraftClient("MCCOpenGLTest");
            var cameraMotivator = new CameraMotivatorRenderer(_camera)
            {
                Controlable = true,
                Type = CameraType.Fps,
                PositionInput = this.CreateKeyAxisInput(
                    positionXKey: Keys.D,
                    positionYKey: Keys.Space,
                    positionZKey: Keys.S,
                    negativeXKey: Keys.A,
                    negativeYKey: Keys.LeftShift,
                    negativeZKey: Keys.W
                ).CreateSmoothAxisInput(),
                RotationInput = this.CreateKeyAxisInput(
                    positionXKey: Keys.Right,
                    positionYKey: Keys.Down,
                    negativeXKey: Keys.Left,
                    negativeYKey: Keys.Up
                ).CreateSmoothAxisInput()
            };
            this.AddRenderObject(new WindowViewportInvoker(this));
            this.AddUpdater(cameraMotivator);
            this.AddRenderObject(_worldRenderer);
            this.AddUpdater(() => Title = _camera.Position.ToString());
            this.AddIntervalTicker(20, () =>
            {
                _worldRenderer.MarkUpdate();
            });
            _serverAddress = serverAddress;
            _serverPort = serverPort;
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

            _atlases = textureBuilder.Build();

            _logger.Info("Finished building texture atlases.");

            _clientThread.Invoke(() =>
            {
                _client.Disconnected += (sender, e) =>
                {
                    _logger.Warn(e);
                    _logger.Info("Relogin in 5 seconds...");
                    _ = Task.Delay(5000).Then(() => _client.Connect(_serverAddress, _serverPort)).HandleException(ex => _logger.Warn(ex));
                };
                _client.ChatReceived += (sender, e) =>
                {
                    _logger.Info(e);
                };

                _ = _client.ServerListPing(_serverAddress, _serverPort).Then(async result =>
                {
                    _logger.Info(result.GetPropertyInfoString());
                    await Task.CompletedTask;
                });

                _ = _client.Connect(_serverAddress, _serverPort).HandleException(ex => _logger.Warn(ex));

                while (true)
                {
                    var input = Console.ReadLine().Trim();
                    if (input.Length == 0)
                        continue;
                    if (input == "#exit")
                    {
                        _ = _client.Disconnect().HandleException(ex => _logger.Warn(ex));
                        return;
                    }
                    _ = _client.SendChatMessage(input).HandleException(ex => _logger.Warn(ex));
                }
            }, async: true);

            base.OnBeforeInitalizers(sender, e);
        }

        private Minecraft.Numerics.Vector3d _lastServerPosition;

        protected override void OnBeforeUpdaters(object sender, EventArgs e)
        {
            _camera.Aspect = (float)ClientSize.X / ClientSize.Y;
            _viewMatrixProvider.Calculate();
            _perspectiveTransformProvider.Calculate();

            if (_client.IsJoined)
            {
                foreach (var entity in _client.GetWorld().GetEntities())
                {
                    var x = (int)entity.Position.X;
                    var y = (int)entity.Position.Y;
                    var z = (int)entity.Position.Z;
                    if (!_world.HasChunk(x >> 0x04, z >> 0x04))
                    {
                        _world.AddChunk(new EmptyChunk()
                        {
                            World = _world,
                            X = x >> 0x04,
                            Z = z >> 0x04
                        });
                    }
                    _world.SetBlock(x, y, z, "diamond_block");
                    _camera.LookAt((x, y, z));
                }

                var player = _client.GetPlayer();

                _worldRenderer.CenterChunkX = player.ServerChunkX;
                _worldRenderer.CenterChunkZ = player.ServerChunkZ;

                if (!_lastServerPosition.Equals(player.Position))
                {
                    (double x, double y, double z) = player.Position;
                    _camera.Position = new Vector3((float)x, (float)y, (float)z);
                }
            }


            base.OnBeforeUpdaters(sender, e);
        }

        protected override void OnBeforeTickers(object sender, EventArgs e)
        {
            if (_client.IsJoined)
            {
                var player = _client.GetPlayer();
                var delta = _lastServerPosition;
                delta.Delta(player.Position);
                if (delta.LengthPow2 <= 15.0F)
                {
                    (float x, float y, float z) = _camera.Position;
                    player.GetPositionHandler().SetPosition((x, y, z), true);
                }
            }
            base.OnBeforeTickers(sender, e);
        }
    }
}
