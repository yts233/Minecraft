using System;
using System.Collections.Generic;
using Minecraft.Graphics;
using Minecraft.Graphics.Arraying;
using OpenTK.Mathematics;

class ASCIIVertexProvider : IVertexArrayProvider<TestVertex>, ICalculator
{
    public Color4 Color { get; set; }
    public Vector2 Offset { get; set; }
    public string Value { get; set; }
    private uint[] _indices = Array.Empty<uint>();
    private TestVertex[] _vertices = Array.Empty<TestVertex>();

    private static readonly float[] Vertices =
    {
        0F, 0F, 0F, 0F,
        1F, 0F, 1F, 0F,
        1F, 1F, 1F, 1F,
        0F, 1F, 0F, 1F
    };

    private static readonly uint[] Indices =
    {
        0,1,2,
        0,2,3
    };

    private void AddChar(char c, List<uint> indices, List<TestVertex> vertices, uint arrow)
    {
        if (c > 255)
            throw new ArgumentOutOfRangeException(nameof(c));
        var arrow3 = arrow * 4;
        var arrow2 = 0;
        for (int i = 0; i < 4; i++)
        {
            vertices.Add(new TestVertex
            {
                X = (arrow + Vertices[arrow2++]) * 64,
                Y = Vertices[arrow2++] * 64,
                R = Color.R,
                G = Color.G,
                B = Color.B,
                A = Color.A,
                U = 0.0625F * ((c & 0x0f) + Vertices[arrow2++]),
                V = 1 - 0.0625F * ((c >> 4) + Vertices[arrow2++])
            });
        }
        for (int i = 0; i < 6; i++)
            indices.Add(arrow3 + Indices[i]);
    }

    public void Calculate()
    {
        var indices = new List<uint>();
        var vertices = new List<TestVertex>();
        uint arrow = 0;
        foreach (char c in Value)
        {
            AddChar(c, indices, vertices, arrow++);
        }

        _indices = indices.ToArray();
        _vertices = vertices.ToArray();
    }

    public IEnumerable<uint> GetIndices()
    {
        return _indices;
    }

    public IEnumerable<VertexAttributePointer> GetPointers()
    {
        yield return new VertexAttributePointer()
        {
            Index = 0,
            Normalized = false,
            Offset = 0,
            Type = VertexAttribePointerType.Float,
            Size = 2
        };
        yield return new VertexAttributePointer()
        {
            Index = 1,
            Normalized = false,
            Offset = 2 * sizeof(float),
            Type = VertexAttribePointerType.Float,
            Size = 4
        };
        yield return new VertexAttributePointer()
        {
            Index = 2,
            Normalized = false,
            Offset = 6 * sizeof(float),
            Type = VertexAttribePointerType.Float,
            Size = 2
        };
    }

    public IEnumerable<TestVertex> GetVertices()
    {
        return _vertices;
    }
}