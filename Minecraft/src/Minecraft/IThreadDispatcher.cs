using System;

namespace Minecraft
{
    public interface IThreadDispatcher : IDisposable
    {
        bool IsRunning { get; }
        string ThreadName { get; set; }
        bool IsBackground { get; set; }

        void Start();
        void Stop();
        void Kill();
        void Invoke(Action action, bool async = false);
    }
}
