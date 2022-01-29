using Minecraft.Graphics.Arraying;
using Minecraft.Resources.Fonts;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Renderers.UI
{
    public class TextHudObject : IHudObject
    {
        internal bool TextUpdated = true;
        internal bool VertexUpdated = false;
        internal bool ModelUpdated = true;
        internal IElementArrayHandle EAH;
        internal TextVertexProvider TVP;
        internal Font RenderFont;
        internal Matrix4 Model;
        private string _text;
        private float _multiLineWidth = 0;
        private Vector2 _fontScale = (16, 16);
        private Vector3 _position;
        private Color4 _color = Color4.White;

        public Font Font
        {
            get => RenderFont;
            set
            {
                RenderFont = value;
                TextUpdated = true;
            }
        }

        public Color4 Color
        {
            get => _color;
            set
            {
                _color = value;
                TextUpdated = true;
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                TextUpdated = true;
            }
        }

        public float MultiLineWidth
        {
            get => _multiLineWidth;
            set
            {
                _multiLineWidth = value;
                TextUpdated = true;
            }
        }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                ModelUpdated = true;
            }
        }

        public Vector2 FontScale
        {
            get => _fontScale;
            set
            {
                _fontScale = value;
                ModelUpdated = true;
            }
        }
    }
}
