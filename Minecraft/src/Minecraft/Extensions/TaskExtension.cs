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
                Logger.GetLogger<T>().Error(exception);
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
                Logger.GetLogger<T>().Error(exception);
            }
        }

        public static async Task Then(this Task runningTask, Func<Task> then)
        {
            await runningTask;
            await then();
        }

        public static async Task Then<TArg>(this Task<TArg> runningTask, Func<Task> then)
        {
            await runningTask;
            await then();
        }

        public static async Task<TResult> Then<TResult>(this Task runningTask, Func<Task<TResult>> then)
        {
            await runningTask;
            return await then();
        }

        public static async Task<TResult> Then<TArg, TResult>(this Task<TArg> runningTask, Func<Task<TResult>> then)
        {
            await runningTask;
            return await then();
        }

        public static async Task<TResult> Then<TArg, TResult>(this Task<TArg> runningTask, Func<TArg, Task<TResult>> then)
        {
            return await then(await runningTask);
        }

        public static async Task Then<TArg>(this Task<TArg> runningTask, Func<TArg, Task> then)
        {
            await then(await runningTask);
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