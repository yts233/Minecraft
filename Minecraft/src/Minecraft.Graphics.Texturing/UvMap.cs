using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Texturing
{
    public class UvMap : EmptyTexture, IReadOnlyDictionary<object, Box2>
    {
        private readonly bool[,] _spaceMap = new bool[64, 64];
        private readonly Dictionary<object, Box2i> _uvDictionary = new Dictionary<object, Box2i>();

        private static Box2 TranslateBox(Box2i value)
        {
            return new Box2(
                value.Min.X / 1024F,
                value.Min.Y / 1024F,
                (value.Max.X) / 1024F,
                (value.Max.Y) / 1024F);
        }

        public UvMap() : base(1024, 1024)
        {
        }

        public void Add(Image image, object key)
        {
            var w = image.Width;
            var h = image.Height;
            var bw = w >> 4;
            var bh = h >> 4;
            if (bw == 0) bw = 1;
            if (bh == 0) bh = 1;

            for (var y = 0; y < 64; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    if (_spaceMap[y, x])
                        continue;

                    var ex = x + bw;
                    var ey = y + bh;
                    if (ex > 64)
                    {
                        x = 64;
                        continue;
                    }

                    if (ey > 64)
                        throw new TextureException("this map is unable to contain this image");

                    var next = false;
                    for (var dy = y; dy < ey; dy++)
                    {
                        if (next) break;
                        for (var dx = x; dx < ex; dx++)
                        {
                            if (!_spaceMap[dy, dx]) continue;
                            next = true;
                            break;
                        }
                    }

                    for (var dy = y; dy < ey; dy++)
                    {
                        for (var dx = x; dx < ex; dx++)
                        {
                            _spaceMap[dy, dx] = true;
                        }
                    }

                    if (next)
                        continue;

                    var bx = x << 4;
                    var by = y << 4;
                    var b = new Box2i(bx, by, bx + w, by + h);
                    _uvDictionary.Add(key, b);
                    this.SubImage(image, bx, by);
                    //Logger.Debug<UvMap>($"Added UvMap: {b} {key}");
                    return;
                }
            }

            throw new TextureException("this map is full");
        }

        public void Remove(object key)
        {
            var b = _uvDictionary[key];
            var x = b.Min.X;
            var y = b.Min.Y;
            var ex = b.Max.X;
            var ey = b.Max.Y;
            for (var dy = y; dy < ey; dy++)
            {
                for (var dx = x; dx < ex; dx++)
                {
                    _spaceMap[dy, dx] = false;
                }
            }

            _uvDictionary.Remove(key);
        }

        public IEnumerator<KeyValuePair<object, Box2>> GetEnumerator()
        {
            return _uvDictionary.Select(kvp =>
                    new KeyValuePair<object, Box2>(kvp.Key, TranslateBox(kvp.Value)))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _uvDictionary.Count;

        public bool ContainsKey(object key)
        {
            return _uvDictionary.ContainsKey(key);
        }

        public bool TryGetValue(object key, out Box2 value)
        {
            var returnValue = _uvDictionary.TryGetValue(key, out var tmpValue);
            value = returnValue ? TranslateBox(tmpValue) : new Box2();
            return returnValue;
        }

        public Box2 this[object key]
        {
            get
            {
                var value = _uvDictionary[key];
                return new Box2(
                    value.Min.X / 1024F,
                    value.Min.Y / 1024F,
                    value.Max.X / 1024F,
                    value.Max.Y / 1024F);
            }
        }

        public IEnumerable<object> Keys => _uvDictionary.Keys;

        public IEnumerable<Box2> Values => _uvDictionary.Select(kvp =>
            new Box2(
                kvp.Value.Min.X / 1024F,
                kvp.Value.Min.Y / 1024F,
                kvp.Value.Max.X / 1024F,
                kvp.Value.Max.Y / 1024F));
    }
}