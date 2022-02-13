using Minecraft.Graphics.Arraying;

namespace Demo.MCGraphics2D
{
    public class BoxObject
    {
        private IElementArrayHandle _eah;
        public void GenerateMeshes()
        {
            _eah = new VertexArray<Col2dVertex>(new Col2dVertex[]
            {
                new Col2dVertex{Position=(0F,0F),Color=(1F,1F,1F,1F) },
                new Col2dVertex{Position=(1F,0F),Color=(1F,1F,1F,1F) },
                new Col2dVertex{Position=(1F,1F),Color=(1F,1F,1F,1F) },
                new Col2dVertex{Position=(0F,1F),Color=(1F,1F,1F,1F) }
            }, Col2dShader.GetPointers()).ToElementArray(new uint[]
            {
                0,1,2,3,0
            }).GetHandle();
        }

        public IElementArrayHandle GetElementArrayHandle()
        {
            return _eah;
        }
    }
}
