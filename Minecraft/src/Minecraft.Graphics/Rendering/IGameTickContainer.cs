using System;
using System.Collections.Generic;

namespace Minecraft.Graphics.Rendering
{
    public interface IGameTickContainer
    {
        /// <summary>
        /// 滴答器
        /// </summary>
        /// <remarks>请不要在这里运行有关渲染和着色器相关代码</remarks>
        ICollection<ITickable> Tickers { get; }

        event EventHandler TickTimerStarted;
        event EventHandler BeforeTickers;
        event EventHandler AfterTickers;
    }
}