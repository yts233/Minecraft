using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Transforming;
using Minecraft.Resources;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.Environments.DayLight
{
    public class DayLightRenderer : ICompletedRenderer
    {
        private readonly IMatrixProvider<Matrix4,Vector4> _viewMatrix;
        private readonly IMatrixProvider<Matrix4,Vector4> _projectionMatrix;
        private readonly IAssetProvider _resource;
        private ITexture2DAtlas _textureAtlas;
        private int _dayTime;

        public DayLightRenderer(IMatrixProvider<Matrix4,Vector4> viewMatrix, IMatrixProvider<Matrix4,Vector4> projectionMatrix, IAssetProvider resource)
        {
            _viewMatrix = viewMatrix;
            _projectionMatrix = projectionMatrix;
            _resource = resource;
        }

        public int DayTime
        {
            get => _dayTime;
            set => _dayTime = value % 24000;
        }

        public void Initialize()
        {
            var textureAtlasBuilder = new TextureAtlasBuilder();
            using var moonStream = _resource[AssetType.Texture, "minecraft:environment/moon_phases.png"]
                .OpenRead();
            using var sunStream = _resource[AssetType.Texture, "minecraft:environment/sun.png"].OpenRead();
            textureAtlasBuilder.Add("moon", new Image(moonStream));
            textureAtlasBuilder.Add("sun", new Image(sunStream));
            for (var i = 0; i < 8; i++)
            {
                textureAtlasBuilder.Add("moon", $"moon_{i}", (i & 0b11) << 1, i >> 0x01, 2, 2);
            }

            _textureAtlas = textureAtlasBuilder.Build();
        }

        public void Render()
        {
            
        }

        public void Update()
        {
            
        }

        public void Bind()
        {
            
        }

        public void Tick()
        {
            _dayTime++;
            if (_dayTime == 24000)
                _dayTime = 0;
        }

        public void Dispose()
        {
            
        }
    }
}