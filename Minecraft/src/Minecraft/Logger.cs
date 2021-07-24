using System;
using System.Collections.Generic;
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

        public static async Task Exception<T>(Exception exception)
        {
            await Fatal<T>($"Unhandled exception. {exception}");
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

        public static async Task LogMemory()
        {
            await Info<MemoryMonitor>(
                 $"Memory: {AppDomain.CurrentDomain.MonitoringSurvivedMemorySize >> 20} / {AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize >> 20} MB");
        }

        private static async void HandleException(Exception exception)
        {
            await Exception<ExceptionHandler>(exception);
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
        public static async Task Log<T>(object log, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    if (!_logFatal) return;
                    break;
                case LogLevel.Error:
                    if (!_logError) return;
                    break;
                case LogLevel.Warn:
                    if (!_logWarn) return;
                    break;
                case LogLevel.Info:
                    if (!_logInfo) return;
                    break;
                case LogLevel.Debug:
                    if (!_logDebug) return;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(null, nameof(level));
            }
            if (Current == null)
                return;
            var threadName = Thread.CurrentThread.Name;
            await Task.Yield();
            lock (Current.Output)
            {
                Current.Log<T>(DateTime.Now, level, log.ToString(), threadName);
            }
        }

        /// <summary>
        ///     记录Info日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public static async Task Info<T>(object log)
        {
            await Log<T>(log, LogLevel.Info);
        }

        /// <summary>
        ///     记录Warn日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public static async Task Warn<T>(object log)
        {
            await Log<T>(log, LogLevel.Warn);
        }

        /// <summary>
        ///     记录Error日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public static async Task Error<T>(object log)
        {
            await Log<T>(log, LogLevel.Error);
        }

        /// <summary>
        ///     记录Fatal日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public static async Task Fatal<T>(object log)
        {
            await Log<T>(log, LogLevel.Fatal);
        }

        /// <summary>
        ///     记录Debug日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <typeparam name="T">记录日志的对象信息</typeparam>
        public static async Task Debug<T>(object log)
        {
            await Log<T>(log, LogLevel.Debug);
        }

        #endregion

        private static readonly ICollection<string> _disabledLogger = new HashSet<string>();

        public static void DisableLogger(string typeName)
        {
            _disabledLogger.Add(typeName);
        }

        public static void EnableLogger(string typeName)
        {
            _disabledLogger.Remove(typeName);
        }

        private static bool _logDebug = true;
        private static bool _logInfo = true;
        private static bool _logWarn = true;
        private static bool _logError = true;
        private static bool _logFatal = true;

        public static void SetLogLevel(LogLevel level, bool isLogged)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    _logFatal = isLogged;
                    return;
                case LogLevel.Error:
                    _logError = isLogged;
                    return;
                case LogLevel.Warn:
                    _logWarn = isLogged;
                    return;
                case LogLevel.Info:
                    _logInfo = isLogged;
                    return;
                case LogLevel.Debug:
                    _logDebug = isLogged;
                    return;
                default:
                    throw new ArgumentOutOfRangeException(null, nameof(level));
            }
        }
    }
}