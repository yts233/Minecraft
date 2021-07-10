using System.Collections.Generic;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public class Transformer : ITransformable
    {
        public Transformer()
        {
        }

        public Transformer(ITransformable transformer)
        {
            BaseTransformer = transformer;
        }

        private ITransformable BaseTransformer { get; }

        public Vector4 Transform(Vector4 vector)
        {
            return BaseTransformer?.Transform(vector) ?? vector;
        }

        public IEnumerable<Vector4> Transform(IEnumerable<Vector4> vectors)
        {
            foreach (var vector in vectors) yield return Transform(vector);
        }
    }
}