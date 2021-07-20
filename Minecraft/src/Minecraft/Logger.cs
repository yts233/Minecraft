using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minecraft
{
    /// <summary>
    ///     日志记录器
    /// </summary>
    /// <remarks>异步记录日志，请在程序结束前调用<see cref="WaitForLogging"/></remarks>
    public class Logger
    {
        /// <summary>
        ///     创建一个记录器
        /// </summary>
        /// <param name="output">输出对象</param>
        public Logger(TextWriter output)
        {
            Output = output;
        }

        /// <summary>
        ///     记录器的输出对象
        /// </summary>
        public TextWriter Output { get; }


        /// <summary>
        ///     当前默认的记录器
        /// </summary>
        public static Logger Current { get; set; } = new ConsoleLogger();

        /// <summary>
        ///     记录日志
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="log">日志</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public void Log<T>(LogLevel level, string log)
        {
            Log<T>(DateTime.Now, level, log, Thread.CurrentThread.Name);
        }

        /// <summary>
        ///     记录日志
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="level">等级</param>
        /// <param name="log">日志</param>
        /// <param name="threadName">线程名</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public virtual void Log<T>(DateTime time, LogLevel level, string log,
            string threadName = null)
        {
            var @string = new StringBuilder();
            @string.Append($"[{time:H:mm:ss}] [{typeof(T).Name}]");
            @string.Append(
                $" [{threadName ?? "UnnamedThread"}/{level.ToString().ToUpper()}]");
            @string.Append($": {log}");
            Output.WriteLine(@string);
        }

        public static void Exception<T>(Exception exception)
        {
            Fatal<T>($"Unhandled exception. {exception}");
        }

        /// <summary>
        ///     给当前线程所在的应用域设置异常记录器
        /// </summary>
        public static void SetExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                HandleException(args.ExceptionObject as Exception);
            };
        }

        public static void EnableMemoryMonitor()
        {
            AppDomain.MonitoringIsEnabled = true;
        }

        public static void LogMemory()
        {
            Info<MemoryMonitor>(
                $"Memory: {AppDomain.CurrentDomain.MonitoringSurvivedMemorySize >> 20} / {AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize >> 20} MB");
        }

        private static void HandleException(Exception exception)
        {
            Exception<ExceptionHandler>(exception);
            WaitForLogging();
#if !DEBUG
            Environment.Exit(1);
#endif
        }

        public static void SetThreadName(string name)
        {
            Thread.CurrentThread.Name = name;
        }

        public static void WaitForLogging()
        {
            lock (Current.Output)
            {
            }
        }

        private delegate void ExceptionHandler();

        private delegate void MemoryMonitor();

        #region Log

        /// <summary>
        ///     记录日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="level">等级</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public static void Log<T>(object log, LogLevel level)
        {
            if (Current == null)
                return;
            var threadName = Thread.CurrentThread.Name; 
            Task.Run(() =>
            {
                lock (Current.Output)
                {
                    Current.Log<T>(DateTime.Now, level, log.ToString(), threadName);
                }
            });
        }

        /// <summary>
        ///     记录Info日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public static void Info<T>(object log)
        {
            Log<T>(log, LogLevel.Info);
        }

        /// <summary>
        ///     记录Warn日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public static void Warn<T>(object log)
        { 
            Log<T>(log, LogLevel.Warn);
        }

        /// <summary>
        ///     记录Error日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public static void Error<T>(object log)
        {
            Log<T>(log, LogLevel.Error);
        }

        /// <summary>
        ///     记录Fatal日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public static void Fatal<T>(object log)
        {
            Log<T>(log, LogLevel.Fatal);
        }

        /// <summary>
        ///     记录Debug日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public static void Debug<T>(object log)
        {
            Log<T>(log, LogLevel.Debug);
        }

        #endregion
    }
}