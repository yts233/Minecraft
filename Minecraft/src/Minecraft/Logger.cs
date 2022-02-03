#define EnableLogTask
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Minecraft
{
    public delegate void LogOutputDelegate(string log, LogLevel level, string threadName, Type senderType, DateTime time);

    public sealed class Logger
    {
        private Logger()
        {
        }

        private static int _logThreadId = 0;
        private static readonly object _logThreadLock = new object();

        private delegate void MemoryMonitor();
        private delegate void CrashHandler();
        private static LogOutputDelegate _output;
        private static readonly Logger<MemoryMonitor> _memoryLogger = GetLogger<MemoryMonitor>();
        private static readonly Logger<CrashHandler> _exceptLogger = GetLogger<CrashHandler>();
#if EnableLogTask
        private static readonly Queue<(string log, LogLevel level, string threadName, Type senderType, DateTime time)> _logQueue = new Queue<(string log, LogLevel level, string threadName, Type senderType, DateTime time)>();
#else
        private static readonly object _logQueue = new object();
#endif
        static Logger()
        {
            _output = (log, level, threadName, senderType, time) =>
            {
                switch (level)
                {
                    case LogLevel.Fatal:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.Warn:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.Info:
                        Console.ResetColor();
                        break;
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(level), level, null);
                }
                var @string = new StringBuilder();
                @string.Append($"[{time:H:mm:ss}] [{senderType?.FullName}]");
                @string.Append(
                    $" [{threadName}/{level.ToString().ToUpper()}]");
                @string.Append($": {log}");
                Console.WriteLine(@string);

                Console.ResetColor();
            };
#if EnableLogTask
            new Thread(LogTask) { IsBackground = true }.Start();
#endif
        }

        private static int _waitCount = 0;

#if EnableLogTask
        private static void LogTask()
        {
            SetThreadName("LoggingThread");
            var logger = GetLogger<Logger>();
            while (true)
            {
                lock (_logQueue)
                {
                    if (_logQueue.TryDequeue(out var logItem))
                    {
                        _output(logItem.log, logItem.level, logItem.threadName, logItem.senderType, logItem.time);
                    }
                    else if (_waitCount != 0)
                    {
                        // 将等待锁全部移动到就绪锁
                        Monitor.PulseAll(_logQueue);
                    }
                }
                if (_logQueue.Count == 0)
                    Thread.Sleep(1); //cpu break
            }
        }
#endif

        public static Logger<T> GetLogger<T>()
        {
            return new Logger<T>();
        }

        internal static void Output(string log, LogLevel level, string threadName, Type senderType, DateTime time)
        {
#if EnableLogTask
            _logQueue.Enqueue((log, level, threadName, senderType, time));
#else
            new Thread(() =>
            {
                lock (_logQueue)
                {
                    _output(log, level, threadName, senderType, time);
                }
            })
            { IsBackground = true }.Start();
#endif
        }

        public static void SetOutput(LogOutputDelegate outputDelegate)
        {
            if (outputDelegate is null)
            {
                throw new ArgumentNullException(nameof(outputDelegate));
            }
            _output = outputDelegate;
        }

        public static void SetThreadName(string name)
        {
            Thread.CurrentThread.Name = name;
        }

        internal static string GetThreadName()
        {
            var currentThread = Thread.CurrentThread;
            var name = currentThread.Name;
            if (name == null)
            {
                int threadId;
                lock (_logThreadLock)
                {
                    threadId = _logThreadId;
                    _logThreadId++;
                }
                name = $"Worker-{threadId}";
                currentThread.Name = name;
            }
            return name;
        }

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
            var size1 = AppDomain.CurrentDomain.MonitoringSurvivedMemorySize;
            var size2 = AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize;
            var moveBits = 0;
            for (var i = 0; i < 3; i++)
            {
                var size3 = size1 >> 10;
                var size4 = size2 >> 10;
                if (size3 == 0 || size4 == 0)
                {
                    break;
                }
                size1 = size3;
                size2 = size4;
                moveBits++;
            }
            var unit = moveBits switch
            {
                0 => "B",
                1 => "KB",
                2 => "MB",
                _ => null
            };
            _memoryLogger.Info($"Memory: {size1} / {size2} {unit}");
        }

        private static void HandleException(Exception exception)
        {
            WaitForLogging();
            _exceptLogger.Fatal($"Oops, the program crashes due to an unhanded exception. This could be an error, or you are not running the program correctly. Here are some details for the unhandled exception.\n\n{exception}\n\nYou called for help, but nobody comes. =)");
            WaitForLogging();
#if !DEBUG //防止调试器的异常处理不起作用
            Environment.Exit(1);
#endif
        }

#if !EnableLogTask
        [Obsolete("no use")]
#endif
        public static void WaitForLogging()
        {
#if EnableLogTask
            lock (_logQueue)
            {
                _waitCount++;
                // 进入等待锁
                Monitor.Wait(_logQueue);
                _waitCount--;
            }
#endif
        }
    }

    /// <summary>
    /// 日志记录器
    /// </summary>
    /// <typeparam name="T">记录器所在的对象</typeparam>
    /// <remarks>异步记录日志，请在程序结束前调用<see cref="WaitForLogging"/></remarks>
    public sealed class Logger<T>
    {
        #region Instance

        private readonly Type _senderType;

        internal Logger()
        {
            _senderType = typeof(T);
        }

        public void Log(object log, LogLevel level)
        {
            Log(log, level, Logger.GetThreadName());
        }

        public void Log(object log, LogLevel level, DateTime time)
        {
            Log(log, level, Logger.GetThreadName(), time);
        }

        public void Log(object log, LogLevel level, string threadName)
        {
            Log(log, level, threadName, DateTime.Now);
        }

        public void Log(object log, LogLevel level, string threadName, DateTime time)
        {
            Logger.Output(log.ToString(), level, threadName, _senderType, time);
        }

        public void Info(object log)
        {
            Log(log, LogLevel.Info);
        }

        public void Info(object log, DateTime time)
        {
            Log(log, LogLevel.Info, time);
        }

        public void Info(object log, string threadName)
        {
            Log(log, LogLevel.Info, threadName);
        }

        public void Info(object log, string threadName, DateTime time)
        {
            Log(log, LogLevel.Info, threadName, time);
        }

        public void Warn(object log)
        {
            Log(log, LogLevel.Warn);
        }

        public void Warn(object log, DateTime time)
        {
            Log(log, LogLevel.Warn, time);
        }

        public void Warn(object log, string threadName)
        {
            Log(log, LogLevel.Warn, threadName);
        }

        public void Warn(object log, string threadName, DateTime time)
        {
            Log(log, LogLevel.Warn, threadName, time);
        }

        public void Error(object log)
        {
            Log(log, LogLevel.Error);
        }

        public void Error(object log, DateTime time)
        {
            Log(log, LogLevel.Error, time);
        }

        public void Error(object log, string threadName)
        {
            Log(log, LogLevel.Error, threadName);
        }

        public void Error(object log, string threadName, DateTime time)
        {
            Log(log, LogLevel.Error, threadName, time);
        }

        public void Fatal(object log)
        {
            Log(log, LogLevel.Fatal);
        }

        public void Fatal(object log, DateTime time)
        {
            Log(log, LogLevel.Fatal, time);
        }

        public void Fatal(object log, string threadName)
        {
            Log(log, LogLevel.Fatal, threadName);
        }

        public void Fatal(object log, string threadName, DateTime time)
        {
            Log(log, LogLevel.Fatal, threadName, time);
        }

        public void Debug(object log)
        {
            Log(log, LogLevel.Debug);
        }

        public void Debug(object log, DateTime time)
        {
            Log(log, LogLevel.Debug, time);
        }

        public void Debug(object log, string threadName)
        {
            Log(log, LogLevel.Debug, threadName);
        }

        public void Debug(object log, string threadName, DateTime time)
        {
            Log(log, LogLevel.Debug, threadName, time);
        }

        /// <summary>
        /// Log "Hello Program"
        /// </summary>
        /// <param name="programName"></param>
        /// <remarks>Everything starts with "Hello world"</remarks>
        public void HelloWorld(string programName = "World")
        {
            Info($"Hello {programName}");
        }

        #endregion
    }
}