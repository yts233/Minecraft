using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Minecraft
{
    public static class ThreadHelper
    {
        public static IThreadDispatcher CreateDispatcher(string threadName = null, bool isBackground = false)
        {
            return new ThreadDispatcher()
            {
                ThreadName = threadName,
                IsBackground = isBackground
            };
        }

        public static Thread StartThread(ThreadStart callback, string threadName = null, bool isBackground = false)
        {
            var thread = new Thread(callback)
            {
                IsBackground = isBackground
            };
            if (threadName != null)
                thread.Name = threadName;
            thread.Start();
            return thread;
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
