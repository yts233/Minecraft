using System;
using System.Linq;
using System.Timers;
using Minecraft;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Renderers.Debuggers.Axis;
using Minecraft.Graphics.Renderers.Environments.Clouding;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Transforming;
using Minecraft.Graphics.Windowing;
using Minecraft.Input;
using Minecraft.Resources;
using Minecraft.Resources.Vanilla.VillageAndPillage;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using static OpenTK.Mathematics.MathHelper;

namespace Test.OpenGL.Test
{
    internal class Program
    {
        private delegate void RenderMonitor();
        private delegate void TickMonitor();
        private static void Main(string[] args)
        {
            // 设置记录器
            Logger.SetExceptionHandler();
            Logger.EnableMemoryMonitor();
            Logger.SetThreadName("MainThread");
            Logger.Info<Program>("Hello Minecraft!");


            // 加载资源和窗体
            var resource = new VanillaResource();
            var window = new RenderWindow
            {
                IsFullScreen = true,
                PointerGrabbed = true,
                RenderFrequency = 30
            };
            var windowViewportInvoker = new WindowViewportInvoker(window);
            IEye eye = new Eye
            {
                Position = (-1, 0, -1),
                DepthFar = 2048F
            };
            eye.LookAt((0F, 0F, 0F));
            var viewProvider = new ViewTransformProvider(eye);
            var projectionProvider = new PerspectiveTransformProvider(eye);
            var cloudRenderer = new CloudRenderer(eye, viewProvider, projectionProvider, resource);
            var axisRenderer = new AxisRenderer(viewProvider, projectionProvider);

            // 设置视图
            windowViewportInvoker.SizeChanged += e =>
            {
                var (width, height) = e;
                eye.Aspect = (float)width / height;
                projectionProvider.CalculateMatrix();
            };
            var mouseDelta = Vector2.Zero;


            // 设置性能记录
            var frames = 0;
            var updates = 0;
            var ticks = 0;
            int fps;
            int ups;
            int tps;
            const double renderWarnTime = 1;
            var renderTimer = new Timer(1000);
            renderTimer.Elapsed += (_, _) =>
            {
                ups = updates;
                fps = frames;
                tps = ticks;
                updates = 0;
                frames = 0;
                ticks = 0;
                window.Title = $"Render Window - FPS: {fps} ({ups})  TPS: {tps}";
                Logger.SetThreadName("RenderInfoThread");
                Logger.Info<RenderMonitor>($"FPS: {fps} ({ups})");
                Logger.Info<TickMonitor>($"TPS: {tps}");
            };
            renderTimer.Start();
            var memTimer = new Timer(5000);
            memTimer.Elapsed += (_, _) =>
            {
                Logger.SetThreadName("MemInfoThread");
                Logger.LogMemory();
            };
            memTimer.Start();

            //更新记录器
            window.AddUpdater(() =>
                {
                    updates++;

                    if (window.PreviousRenderTime >= renderWarnTime)
                        Logger.Warn<Program>(
                            $"Render time: {window.PreviousRenderTime}");
                    if (window.PreviousUpdateTime >= renderWarnTime)
                        Logger.Warn<Program>(
                            $"Update time: {window.PreviousUpdateTime}");
                })
                .AddRenderer(() => frames++);

            // 初始化窗口
            window.AddObject(new WindowInitializer());

            // 更新视图
            window.AddObject(windowViewportInvoker);

            TestShader testShader = null;
            IRenderable testRenderer = null;
            UvAnimation animation = null;
            // 初始化着色器和纹理
            window.AddInitializer(() =>
                {
                    testShader = new TestShader();
                    //testRenderer.BaseRenderer = new TestVertexArrayProvider().ToVertexArray();
                    var time1 = DateTime.Now;
                    var assets = resource.GetAssets().Where(asset =>
                        asset.Type == AssetType.Texture &&
                        asset.Name.StartsWith("block/") &&
                        asset.Name.EndsWith(".png"));
                    var textureBuilder = new TextureAtlasBuilder();
                    var i = 0;
                    foreach (var asset in assets)
                    {
                        using var stream = asset.OpenRead();
                        Logger.Info<Program>(asset.FullName);
                        var bImg = new Image(stream);
                        var isSingle = bImg.FrameCount == 1;
                        var q = 0;
                        foreach (var image in bImg)
                        {
                            textureBuilder.Add(isSingle ? asset.FullName : $"{asset.FullName}{{{q}}}", image);
                            i++;
                            q++;
                            if (i == 4096)
                                break;
                        }

                        if (i == 4096)
                            break;
                    }

                    var texture = textureBuilder.Build();

                    var provider = new TestVertexArrayProvider();

                    testRenderer = provider.ToElementArray();

                    var arr = provider.ToElementArray().GetHandle();
                    const int j = 32;
                    var uvs = new Box2[j];
                    for (i = 0; i < j; i++)
                        uvs[i] = texture[$"minecraft:block/fire_1.png{{{i}}}"];
                    animation = new UvAnimation(arr, texture, uvs, 1,
                        new UvOffsets(6 * sizeof(float),
                            14 * sizeof(float),
                            22 * sizeof(float),
                            30 * sizeof(float)));
                    Logger.Info<Program>($"Textures loaded {(DateTime.Now - time1).TotalMilliseconds} ms");
                })
                .AddInitializer(cloudRenderer)
                .AddInitializer(axisRenderer);

            // 更新输入设备状态
            window.AddUpdater(() =>
                {
                    /* Mouse */
                    var delta = window.PointerState.Delta;
                    if (delta != Vector2.Zero)
                        mouseDelta += delta * .1F;
                    if (mouseDelta == Vector2.Zero)
                        return;
                    mouseDelta *= .5F;
                    eye.Rotation += (mouseDelta.X, -mouseDelta.Y);
                    if (Abs(mouseDelta.X) <= .5)
                        mouseDelta.X = 0;
                    if (Abs(mouseDelta.Y) <= .5)
                        mouseDelta.Y = 0;
                })
                .AddUpdater(() =>
                {
                    const int movementSpeedDiv = 1;
                    /* Keyboard */
                    if (window.KeyboardState[Keys.W]) eye.Position += eye.Front / movementSpeedDiv;
                    if (window.KeyboardState[Keys.S]) eye.Position -= eye.Front / movementSpeedDiv;
                    if (window.KeyboardState[Keys.A]) eye.Position -= eye.Right / movementSpeedDiv;
                    if (window.KeyboardState[Keys.D]) eye.Position += eye.Right / movementSpeedDiv;
                })
                .AddUpdater(() =>
                {
                    viewProvider.CalculateMatrix(); // very important
                });

            // 渲染
            window.AddObject(new ClearRenderer())
                .AddRenderer(() =>
                {
                    testShader.Use();
                    testShader.View = viewProvider.GetMatrix();
                    testShader.Projection = projectionProvider.GetMatrix();
                })
                .AddRenderer(() =>
                {
                    testShader.Use();
                    testRenderer.Render();
                })
                .AddRenderer(() =>
                {
                    testShader.Use();
                    animation.Bind();
                    animation.Render();
                })
                .AddUpdater(() => { cloudRenderer.Update(); })
                .AddRenderer(() =>
                {
                    cloudRenderer.Bind();
                    cloudRenderer.Render();
                });
                //.AddRenderer(() =>
                //{
                //    GL.Clear(ClearBufferMask.DepthBufferBit);
                //    axisRenderer.Bind();
                //    axisRenderer.Render();
                //});

            window.AddTicker(() => ticks++)
                .AddTicker(() => animation.Tick());

            window.ReloadWindow();
            
            Logger.WaitForLogging();
        }
    }
}