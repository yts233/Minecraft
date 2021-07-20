namespace Minecraft.Graphics.Transforming
{
    public interface IMatrixCalculator<out TMatrix, TVector> : IMatrixProvider<TMatrix, TVector>, ICalculator
        where TMatrix : struct
        where TVector : struct
    {
        void CalculateMatrix();

        void ICalculator.Calculate()
        {
            CalculateMatrix();
        }
    }
}