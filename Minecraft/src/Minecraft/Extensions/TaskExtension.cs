using System;
using System.Threading.Tasks;

namespace Minecraft.Extensions
{
    public static class TaskExtension
    {
        public static async void LogException<T, TTaskResult>(this Task<TTaskResult> runningTask)
        {
            try
            {
                await runningTask;
            }
            catch (Exception exception)
            {
                Logger.Exception<T>(exception);
            }
        }

        public static async void LogException<T>(this Task runningTask)
        {
            try
            {
                await runningTask;
            }
            catch (Exception exception)
            {
                Logger.Exception<T>(exception);
            }
        }
    }
}