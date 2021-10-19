namespace Minecraft.Input
{
    public interface IPointerAxisInput : IAxisInput
    {
        /// <summary>
        /// 鼠标灵敏度
        /// </summary>
        /// <remarks>若为0则计算鼠标坐标，否则为Delta</remarks>
        float Sensibility { get; set; }

        bool ZeroOnInactivate { get; set; }
    }
}