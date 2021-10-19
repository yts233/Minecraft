using System;
using System.Collections.Generic;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Transforming;
using Minecraft.Resources;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.Environments.Clouding
{
    public class CloudRenderer : ICompletedRenderer
    {
        private static Logger<CloudRenderer> _logger = Logger.GetLogger<CloudRenderer>();
        private readonly Vector3[,] _cloudColorMap = new Vector3[256, 256];
        private readonly bool[,] _cloudLayoutMap = new bool[256, 256];

        private CloudShader _shader;
        private IElementArrayHandle _cloudVertexArray;

        public CloudRenderer(IEye eye, IMatrixProvider<Matrix4,Vector4> viewMatrix, IMatrixProvider<Matrix4,Vector4> projectionMatrix,
            Resource resource)
        {
            _eye = eye;
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

        private readonly IEye _eye;
        private readonly IMatrixProvider<Matrix4,Vector4> _projectionMatrix;
        private readonly IMatrixProvider<Matrix4,Vector4> _viewMatrix;

        public void Initialize()
        {
            _logger.Info("Loaded cloud.");
            _shader = new CloudShader();
            _cloudVertexArray = new CloudVertexArrayProvider().ToElementArray().GetHandle();
        }

        private int _minOffsetZ;
        private int _offsetZ;
        private int _cloudDistance;

        public void Render()
        {
            _shader.Use();
            _cloudVertexArray.Bind();

            // clouds
            var centerX = (int) _eye.Position.X / 12;
            var centerZ = (int) _eye.Position.Z / 12 + _offsetZ;
            //var inCloudZ = ;
            var minX = centerX - _cloudDistance;
            var minZ = centerZ - _cloudDistance;
            var maxX = centerX + _cloudDistance;
            var maxZ = centerZ + _cloudDistance;

            // shader
            _shader.Offset = _offsetZ * 12 + _minOffsetZ / 60F;
            _shader.View = _viewMatrix.GetMatrix();
            _shader.Projection = _projectionMatrix.GetMatrix();
            _shader.CenterPosition = _eye.Position;

            for (var z = minZ; z < maxZ; z++)
            {
                for (var x = minX; x < maxX; x++)
                {
                    var cx = x & 0xFF;
                    var cz = z & 0xFF;
                    if (!_cloudLayoutMap[cz, cx])
                        continue;
                    _shader.Color = _cloudColorMap[cz, cx];
                    _shader.Position = (x, z);

                    if (!_cloudLayoutMap[cz, (cx - 1) & 0xFF])
                        _cloudVertexArray.Render(12, 6);
                    if (!_cloudLayoutMap[cz, (cx + 1) & 0xFF])
                        _cloudVertexArray.Render(18, 6);
                    if (!_cloudLayoutMap[(cz - 1) & 0xFF, cx])
                        _cloudVertexArray.Render(24, 6);
                    if (!_cloudLayoutMap[(cz + 1) & 0xFF, cx])
                        _cloudVertexArray.Render(30, 6);
                    _cloudVertexArray.Render(0, 12);
                }
            }
        }

        public void Update()
        {
            _cloudDistance = (int) _eye.DepthFar / 12 + 2;
            _minOffsetZ++;
            if (_minOffsetZ != 720) return;
            _minOffsetZ = 0;
            _offsetZ++;
            if (_offsetZ == 256)
                _offsetZ = 0;
        }

        public void Dispose()
        {
            (_shader as IDisposable)?.Dispose();
            _shader = null;
            _cloudVertexArray?.Dispose();
            _cloudVertexArray = null;
        }

        void ITickable.Tick()
        {
        }
    }
}