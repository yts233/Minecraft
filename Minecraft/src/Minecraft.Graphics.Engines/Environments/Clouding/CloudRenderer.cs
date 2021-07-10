using System;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Transforming;
using Minecraft.Resources;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.Environments.Clouding
{
    public class CloudRenderer : ICompletedRenderer
    {
        private readonly Vector3[,] _cloudColorMap = new Vector3[256, 256];
        private readonly bool[,] _cloudLayoutMap = new bool[256, 256];

        private CloudShader _shader;
        private IElementArrayHandle _cloudVertexArray;

        public CloudRenderer(ICamera camera, IMatrixProvider viewMatrix, IMatrixProvider projectionMatrix,
            Resource resource)
        {
            _camera = camera;
            _viewMatrix = viewMatrix;
            _projectionMatrix = projectionMatrix;
            using var stream = resource.GetAsset(AssetType.Texture, "minecraft:environment/clouds.png").OpenRead();
            var cloudData = new Image(stream).Data;
            for (var y = 0; y < 256; y++)
            {
                for (var x = 0; x < 256; x++)
                {
                    var l = y << 10 | x << 2;
                    var r = cloudData[l];
                    var g = cloudData[l | 0b01];
                    var b = cloudData[l | 0b10];
                    var a = cloudData[l | 0b11];
                    var v = new Vector3(r / 255F, g / 255F, b / 255F);
                    _cloudColorMap[y, x] = v;
                    _cloudLayoutMap[y, x] = a == 255;
                    //Logger.Info<CloudRenderer>($"{v} {a}");
                }
            }
        }

        private readonly ICamera _camera;
        private readonly IMatrixProvider _projectionMatrix;
        private readonly IMatrixProvider _viewMatrix;

        public void Initialize()
        {
            Logger.Info<CloudRenderer>("Loaded cloud.");
            _shader = new CloudShader();
            _cloudVertexArray = new CloudVertexArrayProvider().ToElementArray().GetHandle();
        }

        private int _minOffsetZ;
        private int _offsetZ;


        public void Render()
        {
            // shader
            _shader.Offset = _offsetZ * 12 + _minOffsetZ / 120F;
            _shader.View = _viewMatrix.GetMatrix();
            _shader.Projection = _projectionMatrix.GetMatrix();

            // clouds
            const int cloudDistance = 10;
            var centerX = (int) _camera.Position.X % 12;
            var centerZ = (int) _camera.Position.Z % 12 + _offsetZ;
            var minX = centerX - cloudDistance;
            var minZ = centerZ - cloudDistance;
            var maxX = centerX + cloudDistance;
            var maxZ = centerZ + cloudDistance;

            for (var z = minZ; z < maxZ; z++)
            {
                for (var x = minX; x < maxX; x++)
                {
                    var cx = x & 0xFF;
                    var cz = z & 0xFF;
                    //Logger.Debug<CloudRenderer>((cx, cz, x, z));
                    if (!_cloudLayoutMap[cz, cx])
                        continue;
                    _shader.Color = _cloudColorMap[cz, cx];
                    _shader.Position = (x, z);
                    _cloudVertexArray.Render();
                }
            }
        }

        public void Update()
        {
            _minOffsetZ++;
            if (_minOffsetZ != 1440) return;
            _minOffsetZ = 0;
            _offsetZ++;
            if (_offsetZ == 256)
                _offsetZ = 0;
        }

        public void Bind()
        {
            _shader.Use();

            _cloudVertexArray.Bind();
        }

        public void Dispose()
        {
            (_shader as IDisposable)?.Dispose();
            _cloudVertexArray.Dispose();
            _shader = null;
            _cloudVertexArray = null;
        }

        void ITickable.Tick()
        {
        }
    }
}