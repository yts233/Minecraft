using System.Collections.Generic;
using System.Linq;
using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Rendering;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Texturing
{
    public class UvAnimation : IRenderable, ITickable, IBindable
    {
        public UvAnimation(IVertexArrayHandle vertexArrayHandle, ITexture texture, IEnumerable<Box2> uvCoords,
            int frameTime, UvOffsets offsets)
        {
            _vertexArrayHandle = vertexArrayHandle;
            _texture = texture;
            _uvCoords = uvCoords.ToArray();
            _frameTime = frameTime;
            _offsets = offsets;
            _frameCount = _uvCoords.Length;
        }

        private readonly IVertexArrayHandle _vertexArrayHandle;
        private readonly ITexture _texture;
        private readonly Box2[] _uvCoords;
        private readonly int _frameTime;
        private readonly int _frameCount;
        private readonly UvOffsets _offsets;

        private int _currentTick;
        private int _index;
        private bool _needUpdate = true;

        public void Render()
        {
            if (_needUpdate)
            {
                const int size = sizeof(float) * 2;
                var coord = _uvCoords[_index];
                _vertexArrayHandle.VertexSubData(_offsets.ButtonLeft, size, new[] {coord.Min.X, coord.Min.Y})
                    .VertexSubData(_offsets.ButtonRight, size, new[] {coord.Max.X, coord.Min.Y})
                    .VertexSubData(_offsets.TopLeft, size, new[] {coord.Min.X, coord.Max.Y})
                    .VertexSubData(_offsets.TopRight, size, new[] {coord.Max.X, coord.Max.Y});
                _needUpdate = false;
            }

            _vertexArrayHandle.Render();
        }

        public void Bind()
        {
            _texture.Bind();
            _vertexArrayHandle.Bind();
        }

        public void Tick()
        {
            _currentTick++;
            //Logger.Info<UvAnimation>(_currentTick);
            if (_currentTick < _frameTime) return;
            _currentTick = 0;
            _index++;
            if (_index == _frameCount)
                _index = 0;
            _needUpdate = true;
        }
    }
}