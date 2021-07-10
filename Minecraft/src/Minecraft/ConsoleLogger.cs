using System;

namespace Minecraft
{
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger() : base(Console.Out)
        {
        }

        public override void Log<T>(DateTime time, LogLevel level, string log, string threadName = null)
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

            base.Log<T>(time, level, log, threadName);
            Console.ResetColor();
        }
    }
}