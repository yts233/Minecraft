using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

namespace Minecraft.Graphics.Transforming
{
    public class Transformer<TVector> : ITransformable<TVector> where TVector : struct
    {
        public Transformer()
        {
        }

        public Transformer(ITransformable<TVector> transformer)
        {
            BaseTransformer = transformer;
        }

        private ITransformable<TVector> BaseTransformer { get; }

        public TVector Transform(TVector vector)
        {
            return BaseTransformer?.Transform(vector) ?? vector;
        }

        public IEnumerable<TVector> Transform(IEnumerable<TVector> vectors)
        {
            return vectors.Select(Transform);
        }
    }
}