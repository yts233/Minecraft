using Minecraft.Protocol.Data;
using Minecraft.Text;
using System;
using System.IO;

namespace Test.Protocol.Data.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var stream = new MemoryStream();
            var content = new ByteArray(stream);
            void TestValue<T>(T value) where T : IDataType, new()
            {
                content.Position = 0;
                content.Write(value);
                content.Position = 0;
                var value2 = content.Read<T>();
                Console.WriteLine($"{value}\t->\t{value2}");
            }
            TestValue<Minecraft.Protocol.Data.Boolean>(true);
            TestValue<Minecraft.Protocol.Data.Byte>(123);
            TestValue<Minecraft.Protocol.Data.Double>(123456);
            TestValue<Float>(123456);
            TestValue<Int>(123456);
            TestValue<Long>(123456789);
            TestValue<Short>(1234);
            TestValue<Uuid>(new Minecraft.Uuid(Guid.NewGuid()));
            TestValue<VarInt>(123456);
        }
    }
}