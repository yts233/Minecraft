using Minecraft.Graphics.Rendering;
using Minecraft.Graphics.Texturing;
using Minecraft.Graphics.Transforming;
using Minecraft.Resources;

namespace Minecraft.Graphics.Renderers.Environments.DayLight
{
    public class DayLightRenderer : ICompletedRenderer
    {
        private readonly IMatrixProvider _viewMatrix;
        private readonly IMatrixProvider _projectionMatrix;
        private readonly Resource _resource;
        private TextureAtlas _textureAtlas;
        private int _dayTime;

        public DayLightRenderer(IMatrixProvider viewMatrix, IMatrixProvider projectionMatrix, Resource resource)
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
            _textureAtlas = new TextureAtlas(9, 4);
            using var moonStream = _resource.GetAsset(AssetType.Texture, "minecraft:environment/moon_phases.png")
                .OpenRead();
            using var sunStream = _resource.GetAsset(AssetType.Texture, "minecraft:environment/sun.png").OpenRead();
            _textureAtlas.Add(new Image(moonStream), "moon");
            _textureAtlas.Add(new Image(sunStream), "sun");
            for (var i = 0; i < 8; i++)
            {
                _textureAtlas.Add("moon", $"moon_{i}", (i & 0b11) << 1, i >> 0x01, 2, 2);
            }
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