using Minecraft;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Threading;

namespace Test.OpenGL.Test2
{
    class A
    {
        private int a;

        public void B()
        {
            Logger.GetLogger<A>().Info($"a={a}");
            a = a + 1;
            Logger.GetLogger<A>().Info($"a={a}");
        }

        public void C()
        {
            Logger.GetLogger<A>().Info($"a={a}");
            a = a + 12;
            Logger.GetLogger<A>().Info($"a={a}");
        }
    }
    class Program
    {
        static void Main()
        {
            Logger.SetThreadName("MainThread");
            new Program().Run();
        }

        void Run()
        {
            var a = new A();
            ThreadHelper.NewThread("ThreadA").Invoke(() =>
            {
                a.B();
            });
            ThreadHelper.NewThread("ThreadA").Invoke(() =>
            {
                a.C();
            });

            Logger.WaitForLogging();
        }
    }
}