using System;

namespace Minecraft
{
    public interface IThreadDispatcher : IDisposable
    {
        bool IsRunning { get; }
        string ThreadName { get; set; }
        void Start();
        void Stop();
        void Kill();
        void Invoke(Action action, bool async = false);
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
