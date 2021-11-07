using System;

namespace Minecraft.Visual
{
    public interface IApplicationBuilder
    {
        void AddService<TService, TProvider>()
           where TProvider : class;

        void AddInstanceProvider<T, TProvider>()
           where TProvider : class;
    }
    public interface IServicesProvider
    {
        TService GetService<TService>();

        T CreateInstance<T>();
    }
    public static class Application
    {
        public static IApplicationBuilder CreateBuilder()
        {
            throw new NotImplementedException();
        }


    }
}
