using Minecraft.Graphics.Arraying;
using Minecraft.Graphics.Texturing;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Minecraft.Graphics.Renderers.UI
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct HudVertex
    {
        public float X;
        public float Y;
        public float R;
        public float G;
        public float B;
        public float A;
        public float U;
        public float V;

        public override string ToString()
        {
            return $"({X}, {Y}) ({R}, {G}, {B}, {A}) ({U}, {V})";
        }
    }
    internal class TextVertexProvider : IVertexArrayProvider<HudVertex>
    {
        private readonly TextHudObject _tho;
        private HudVertex[] _vert;
        private uint[] _ind;

        private static readonly float[] Vertices =
        {
            0F,0F,0F,1F,
            1F,0F,1F,1F,
            1F,1F,1F,0F,
            0F,1F,0F,0F
        };

        private static readonly uint[] Indices =
        {
            0,2,1,
            0,3,2
        };

        public TextVertexProvider(TextHudObject tho)
        {
            _tho = tho;
        }

        public void Calculate(ITexture2DAtlas fontTexture)
        {
            var vert = new List<HudVertex>();
            var ind = new List<uint>();
            int line = 0;
            float pixel = 0;
            uint indArrow = 0;
            var font = _tho.RenderFont;
            var color = _tho.Color;
            lock (_tho)
            {
                foreach (char c in _tho.Text)
                {
                    //enter
                    if (c == '\n')
                    {
                        line++;
                        pixel = 0;
                        continue;
                    }

                    //parse char
                    var chri = font.GetChar(c);
                    if (chri == null && (chri = font.GetChar('\0')) == null)
                        continue;

                    var chr = chri.Value;
                    Vector2 size = ((chr.x2 - chr.x1) / (chr.y2 - chr.y1), 1) * _tho.FontScale;

                    //auto enter
                    if (_tho.MultiLineWidth > 0 && pixel + size.X > _tho.MultiLineWidth)
                    {
                        line++;
                        pixel = 0;
                    }

                    var box = fontTexture[chr.file];

                    var x1 = box.Min.X + box.Size.X * chr.x1;
                    var y1 = box.Min.Y + box.Size.Y * (1 - chr.y2);
                    var x2 = box.Min.X + box.Size.X * chr.x2;
                    var y2 = box.Min.Y + box.Size.Y * (1 - chr.y1);

                    //vertices
                    int arrow = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        var vertex = new HudVertex
                        {
                            X = pixel + Vertices[arrow++] * size.X,
                            Y = (line + Vertices[arrow++]) * size.Y,
                            R = color.R,
                            G = color.G,
                            B = color.B,
                            A = color.A,
                            U = Vertices[arrow++] == 0F ? x1 : x2,
                            V = Vertices[arrow++] == 0F ? y1 : y2
                        };
                        vert.Add(vertex);
                    }

                    //indices
                    for (int i = 0; i < 6; i++)
                    {
                        ind.Add(Indices[i] + indArrow);
                    }
                    indArrow += 4;
                    pixel += size.X;
                }
            }
            _vert = vert.ToArray();
            _ind = ind.ToArray();
        }

        public void Clear()
        {
            _vert = null;
            _ind = null;
        }

        public IEnumerable<uint> GetIndices()
        {
            return _ind;
        }

        public IEnumerable<VertexAttributePointer> GetPointers()
        {
            yield return new VertexAttributePointer
            {
                Index = 0,
                Normalized = false,
                Offset = 0,
                Size = 2,
                Type = VertexAttribePointerType.Float
            };
            yield return new VertexAttributePointer
            {
                Index = 1,
                Normalized = false,
                Offset = 2 * sizeof(float),
                Size = 4,
                Type = VertexAttribePointerType.Float
            };
            yield return new VertexAttributePointer
            {
                Index = 2,
                Normalized = false,
                Offset = 6 * sizeof(float),
                Size = 2,
                Type = VertexAttribePointerType.Float
            };
        }

        public IEnumerable<HudVertex> GetVertices()
        {
            return _vert;
        }
    }
}
