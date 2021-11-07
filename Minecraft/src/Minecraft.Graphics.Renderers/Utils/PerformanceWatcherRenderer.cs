using Minecraft.Graphics.Rendering;
using System;
using System.Threading;

namespace Minecraft.Graphics.Renderers.Utils
{
    public class PerformanceWatcherRenderer : ICompletedRenderer
    {
        public int _ticks;
        public int _renders;
        public int _updates;
        private bool _fisrt = true;
        private DateTime _lastTime;
        public int LastRenderTimes { get; private set; }
        public int LastUpdateTimes { get; private set; }
        /// <summary>
        /// Last 20 ticks seconds
        /// </summary>
        public double LastTickTime { get; private set; }

        public void Dispose()
        {
        }

        public void Initialize()
        {
        }

        public void Render()
        {
            lock (_locker)
                _renders++;
        }

        private object _locker = new object();

        public void Tick()
        {
            lock (_locker)
            {
                if (_fisrt)
                {
                    _lastTime = DateTime.Now;
                    _fisrt = false;
                }
                if (++_ticks == 20)
                {
                    LastTickTime = (DateTime.Now - _lastTime).TotalSeconds;
                    LastRenderTimes = _renders;
                    _renders = 0;
                    LastUpdateTimes = _updates;
                    _updates = 0;
                    _ticks = 0;
                }
            }
        }

        public void Update()
        {
            lock (_locker)
                _updates++;
        }
    }
}
