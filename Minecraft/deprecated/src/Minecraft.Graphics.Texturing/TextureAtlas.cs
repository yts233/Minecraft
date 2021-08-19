using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Minecraft.Extensions;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Texturing
{
    public class TextureAtlasBuilder
    {
        private readonly bool[,] _spaceMap = new bool[64, 64];

        private readonly Dictionary<object, (Box2i space, byte[]data, int width, int height)> _imageDictionary =
            new Dictionary<object, (Box2i space, byte[]data, int width, int height)>();

        private readonly Dictionary<object, (object baseKey, Box2i space)> _extraDictionary =
            new Dictionary<object, (object baseKey, Box2i space)>();

        private int _maxHeight = 1;

        public TextureAtlasBuilder Add(object key, Image image)
        {
            if (ContainsKey(key))
                throw new InvalidOperationException("the key is already exist");
            var w = image.Width; // width of the image
            var h = image.Height; // height of the image
            var bw = (w + 15) >> 4; // width in blocks
            var bh = (h + 15) >> 4; // height in blocks

            for (var y = 0; y < 64; y++) // find the space
            {
                for (var x = 0; x < 64; x++)
                {
                    if (_spaceMap[y, x])
                        continue;

                    var ex = x + bw; // end x of block
                    var ey = y + bh; // end y of block

                    if (ey > 64)
                        throw new TextureException("the map is unable to contain the image");
                    if (ex > 64) break;

                    var noSpace = false;
                    for (var dy = y; dy < ey; dy++) // check if the space is valid
                    {
                        if (noSpace) break;
                        for (var dx = x; dx < ex; dx++)
                        {
                            if (!_spaceMap[dy, dx]) continue;
                            noSpace = true;
                            break;
                        }
                    }

                    if (noSpace) continue;

                    for (var dy = y; dy < ey; dy++) // fill the space map
                    {
                        for (var dx = x; dx < ex; dx++)
                        {
                            _spaceMap[dy, dx] = true;
                        }
                    }


                    var px = x << 4; // x in pixels
                    var py = y << 4; // y in pixels
                    var b = new Box2i(px, py, px + w, py + h);
                    _imageDictionary.Add(key, (b, image.Data, image.Width, image.Height));
                    if (_maxHeight < ey) _maxHeight = ey;
                    //Logger.Debug<UvMap>($"Added UvMap: {b} {key}");
                    return this;
                }
            }

            throw new TextureException("the map is full");
        }

        public void Add(object baseKey, object key, int xInUnits, int yInUnits, int widthInUnits, int heightInUnits)
        {
            var uv = _imageDictionary[baseKey].space;
            var x = (xInUnits << 4) + uv.Min.X;
            var y = (yInUnits << 4) + uv.Min.Y;
            var mx = x + (widthInUnits << 4);
            var my = y + (heightInUnits << 4);
            _extraDictionary.Add(key, (baseKey, new Box2i(x, y, mx, my)));
        }

        public void Remove(object key)
        {
            Box2i b;
            bool useExtraDictionary;
            if (_imageDictionary.TryGetValue(key, out var imageInfo))
            {
                b = imageInfo.space;
                useExtraDictionary = false;
            }
            else if (_extraDictionary.TryGetValue(key, out var extraInfo))
            {
                b = extraInfo.space;
                useExtraDictionary = true;
            }
            else throw new KeyNotFoundException();

            if (useExtraDictionary)
                _extraDictionary.Remove(key);
            else
            {
                _imageDictionary.Remove(key);
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

                foreach (var key1 in _extraDictionary
                    .Where(kvp => kvp.Value.baseKey == key)
                    .Select(kvp => kvp.Key))
                {
                    _extraDictionary.Remove(key1);
                }
            }
        }

        public bool ContainsKey(object key)
        {
            return _imageDictionary.ContainsKey(key) || _extraDictionary.ContainsKey(key);
        }

        public TextureAtlas Build()
        {
            var textureAtlas = new TextureAtlas(_imageDictionary, _extraDictionary, 1 << _maxHeight.GetBitsCount());
            textureAtlas.GenerateMipmaps();
            return textureAtlas;
        }
    }

    public class TextureAtlas : EmptyTexture, IReadOnlyDictionary<object, Box2>
    {
        private readonly IReadOnlyDictionary<object, Box2> _spaces;

        public TextureAtlas(IDictionary<object, (Box2i space, byte[]data, int width, int height)> images,
            IDictionary<object, (object baseKey, Box2i space)> extraImages, int heightInBlocks) : base(1024,
            heightInBlocks << 4)
        {
            var spaces = new Dictionary<object, Box2>();
            float height = heightInBlocks << 4;
            Box2 TranslateBox(Box2i value)
            {
                return new Box2(
                    value.Min.X / 1024F,
                    value.Min.Y / height,
                    value.Max.X / 1024F,
                    value.Max.Y / height);
            }

            foreach (var (key, value) in images)
            {
                this.SubImage(value.data, value.space.Min.X, value.space.Min.Y, value.width, value.height);
                spaces.Add(key, TranslateBox(value.space));
            }

            foreach (var (key, value) in extraImages)
            {
                spaces.Add(key, TranslateBox(value.space));
            }

            _spaces = spaces;
        }

        public IEnumerator<KeyValuePair<object, Box2>> GetEnumerator()
        {
            return _spaces.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _spaces.Count;

        public bool ContainsKey(object key)
        {
            return _spaces.ContainsKey(key);
        }

        public bool TryGetValue(object key, out Box2 value)
        {
            return _spaces.TryGetValue(key, out value);
        }

        public Box2 this[object key] => _spaces[key];

        public IEnumerable<object> Keys => _spaces.Keys;

        public IEnumerable<Box2> Values => _spaces.Values;
    }
}