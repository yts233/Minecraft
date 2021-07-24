using System;
using System.Threading.Tasks;

namespace Minecraft.Extensions
{
    public static class TaskExtension
    {
        public static async Task LogException<T, TTaskResult>(this Task<TTaskResult> runningTask)
        {
            try
            {
                await runningTask;
            }
            catch (Exception exception)
            {
                await Logger.Exception<T>(exception);
            }
        }

        public static async Task LogException<T>(this Task runningTask)
        {
            try
            {
                await runningTask;
            }
            catch (Exception exception)
            {
                await Logger.Exception<T>(exception);
            }
        }

        public static async Task HandleException(this Task runningTask, Action<Exception> handler = null)
        {
            try
            {
                await runningTask;
            }
            catch (Exception exception)
            {
                handler?.Invoke(exception);
            }
        }

        public static async Task HandleException<TTaskResult>(this Task<TTaskResult> runningTask, Action<Exception> handler = null)
        {
            try
            {
                await runningTask;
            }
            catch (Exception exception)
            {
                handler?.Invoke(exception);
            }
        }
    }
}