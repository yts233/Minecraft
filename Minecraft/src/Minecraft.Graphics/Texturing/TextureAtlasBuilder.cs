using System;
using System.Collections.Generic;
using System.Linq;
using Minecraft.Extensions;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Texturing
{
    public class TextureAtlasBuilder
    {
        private readonly bool[,] _spaceMap = new bool[64, 64];

        private readonly Dictionary<NamedIdentifier, (Box2i space, byte[] data, int width, int height)> _imageDictionary =
            new Dictionary<NamedIdentifier, (Box2i space, byte[] data, int width, int height)>();

        private readonly Dictionary<NamedIdentifier, (NamedIdentifier baseKey, Box2i space)> _extraDictionary =
            new Dictionary<NamedIdentifier, (NamedIdentifier baseKey, Box2i space)>();

        private int _maxHeight = 1;

        public TextureAtlasBuilder Add(NamedIdentifier key, Image image)
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
                        throw new TextureException("the map is unable to contain more images");
                    if (ex > 64) break;

                    var noSpace = false;
                    for (var dy = y; dy < ey && !noSpace; dy++) // check if the space is valid
                    {
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

        public void Add(NamedIdentifier baseKey, NamedIdentifier key, int xInUnits, int yInUnits, int widthInUnits, int heightInUnits)
        {
            var uv = _imageDictionary[baseKey].space;
            var x = (xInUnits << 4) + uv.Min.X;
            var y = (yInUnits << 4) + uv.Min.Y;
            var mx = x + (widthInUnits << 4);
            var my = y + (heightInUnits << 4);
            _extraDictionary.Add(key, (baseKey, new Box2i(x, y, mx, my)));
        }

        public void Remove(NamedIdentifier key)
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

        public bool ContainsKey(NamedIdentifier key)
        {
            return _imageDictionary.ContainsKey(key) || _extraDictionary.ContainsKey(key);
        }

        public ITextureAtlas Build()
        {
            var textureAtlas = new TextureAtlas(_imageDictionary, _extraDictionary, 1 << _maxHeight.GetBitsCount());
            textureAtlas.GenerateMipmaps2D();
            return textureAtlas;
        }
    }
}