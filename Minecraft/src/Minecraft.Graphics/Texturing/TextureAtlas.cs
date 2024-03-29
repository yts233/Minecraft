using System.Collections;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Texturing
{
    internal class Texture2DAtlas : EmptyTexture2D, ITexture2DAtlas
    {
        private readonly IReadOnlyDictionary<NamedIdentifier, Box2> _spaces;

        public Texture2DAtlas(IDictionary<NamedIdentifier, (Box2i space, byte[] data, int width, int height)> images,
            IDictionary<NamedIdentifier, (NamedIdentifier baseKey, Box2i space)> extraImages, int heightInBlocks) : base(1024,
            heightInBlocks << 4)
        {
            var spaces = new Dictionary<NamedIdentifier, Box2>();
            float height = heightInBlocks << 4;
            const float offset = .0001F;
            Box2 TranslateBox(Box2i value)
            {
                return new Box2(
                    value.Min.X / 1024F + offset,
                    (value.Min.Y + offset * 1024F) / height,
                    value.Max.X / 1024F - offset,
                    (value.Max.Y - offset * 1024F) / height);
            }

            foreach (var (key, value) in images)
            {
                this.SubImage2D(value.data, value.space.Min.X, value.space.Min.Y, value.width, value.height);
                spaces.Add(key, TranslateBox(value.space));
            }

            foreach (var (key, value) in extraImages)
            {
                spaces.Add(key, TranslateBox(value.space));
            }

            _spaces = spaces;
        }

        public IEnumerator<KeyValuePair<NamedIdentifier, Box2>> GetEnumerator()
        {
            return _spaces.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _spaces.Count;

        public bool ContainsKey(NamedIdentifier key)
        {
            return _spaces.ContainsKey(key);
        }

        public bool TryGetValue(NamedIdentifier key, out Box2 value)
        {
            return _spaces.TryGetValue(key, out value);
        }

        public Box2 this[NamedIdentifier key]
        {
            get
            {
                if (_spaces.TryGetValue(key, out var value))
                    return value;
                return new Box2(0.0F, 0.0F, 1.0F, 1.0F);
            }
        }

        public IEnumerable<NamedIdentifier> Keys => _spaces.Keys;

        public IEnumerable<Box2> Values => _spaces.Values;
    }
}