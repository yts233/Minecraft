using System;

namespace Minecraft.Graphics.Rendering
{
    public class CustomTicker : ITickable
    {
        private readonly Action _action;

        public CustomTicker(Action action)
        {
            _action = action;
        }

        public void Tick()
        {
            _action?.Invoke();
        }
    }
    public class TimerTicker : ITickable
    {
        private readonly int _interval;
        private int _ticks;

        private readonly Action _callback;

        /// <summary>
        /// 创建<see cref="TimeTicker"/>实例
        /// </summary>
        /// <param name="interval">ticks</param>
        /// <param name="callback"></param>
        public TimerTicker(int interval, Action callback)
        {
            if (interval <= 0)
                throw new ArgumentOutOfRangeException(nameof(interval), "interval should be greater than 0");
            _interval = interval;
            _callback = callback;
        }

        
        public void Tick()
        {
            if (++_ticks == _interval)
            {
                _callback?.Invoke();
                _ticks = 0;
            }
        }
    }
}