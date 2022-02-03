using System;
using System.Collections.Generic;
using System.Threading;

namespace Minecraft
{
    internal class ThreadDispatcher : IThreadDispatcher
    {
        private static readonly Logger<ThreadDispatcher> _logger = Logger.GetLogger<ThreadDispatcher>();
        private Thread _thread;
        private string _threadName;
        private readonly Queue<(Action action, bool wait)> _executeQueue = new Queue<(Action action, bool wait)>();
        public bool IsRunning => _thread?.IsAlive ?? false;
        private bool _running;
        private bool _disposedValue;
        private bool _isBackground;

        public bool IsBackground
        {
            get => _isBackground;
            set
            {
                _isBackground = value;
            }
        }

        public string ThreadName
        {
            get
            {
                if (_thread != null && _thread.Name != _threadName)
                    _threadName = _thread.Name;
                return _threadName;
            }
            set
            {
                if (_thread != null)
                    _thread.Name = _threadName;
                _threadName = value;
            }
        }

        private readonly object _invokeLock = new object();

        public void Invoke(Action action, bool async = false)
        {
            if (_thread == null)
                Start();
            lock (_executeQueue)
                _executeQueue.Enqueue((action, !async));
            if (!async)
                lock (_invokeLock)
                    Monitor.Wait(_invokeLock);
        }

        private void DispatcherLoop()
        {
            _logger.Info("Thread started.");
            while (true) //execute loop
            {
                if (!_executeQueue.TryDequeue(out var value))
                    if (_running)
                    {
                        Thread.CurrentThread.IsBackground = true;
                        Thread.Sleep(1); // cpu break
                        continue;
                    }
                    else break; // exit the thread when all callbacks are invoked
                Thread.CurrentThread.IsBackground = IsBackground;
                value.action?.Invoke(); // invoke
                if (value.wait)
                    lock (_invokeLock)
                        Monitor.Pulse(_invokeLock);
            }

            // stop
            lock (_stopLock)
            {
                Monitor.PulseAll(_stopLock);
            }
        }

        public void Kill()
        {
            if (!_running)
                return;
            _running = false;
            if (_thread.IsAlive)
                _thread.Interrupt();
            _thread = null;
            lock (_stopLock)
            {
                Monitor.PulseAll(_stopLock);
            }
        }

        public void Start()
        {
            if (_disposedValue)
                throw new ObjectDisposedException(nameof(ThreadDispatcher));
            if (_running)
                return;
            _running = true;
            _thread = new Thread(DispatcherLoop);
            if (_threadName != null)
                _thread.Name = _threadName;
            _thread.IsBackground = true;
            _thread.Start();
        }

        private readonly object _stopLock = new object();
        public void Stop()
        {
            lock (_stopLock)
            {
                if (_thread == null)
                    return;
                _running = false;
                Monitor.Wait(_stopLock); //wait until the thread is stop
                _thread = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    Kill();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                _thread = null;
                _disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ThreadDispatcher()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
#if false
    public class ObjectTransformer : IDisposable
    {
        private readonly Stream _baseStream;
        private readonly BinaryFormatter _formatter = new BinaryFormatter();
        private readonly BinaryReader _binaryReader;
        private readonly BinaryWriter _binaryWriter;

        public ObjectTransformer(Stream baseStream)
        {
            _baseStream = baseStream;
            _binaryReader = new BinaryReader(_baseStream);
            _binaryWriter = new BinaryWriter(_baseStream);
        }

        public void WriteObject(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            using var buffer = new MemoryStream();
            _formatter.Serialize(buffer, obj);
            var data = buffer.ToArray();
            _binaryWriter.Write(data.Length);
            _baseStream.Write(data, 0, data.Length);
        }

        public object ReadObject()
        {
            var length = _binaryReader.ReadInt32();
            var data = new byte[length];
            _baseStream.Read(data, 0, length);
            using var buffer = new MemoryStream(data);
            return _formatter.Deserialize(buffer);
        }

        public void Dispose()
        {
            _baseStream.Dispose();
            _binaryReader.Dispose();
            _binaryWriter.Dispose();
        }
    }
#endif
}
