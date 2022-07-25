using System;
using System.Diagnostics;
using NWebDav.Server.Logging;

namespace NWebDav.Sample.HttpListener.LogAdapters
{
    public class ConsoleAdapter : ILoggerFactory
    {
        private class ConsoleLogger : ILogger
        {
            private readonly Type _type;

            public ConsoleLogger(Type type)
            {
                _type = type;
                Console.ResetColor();
            }

            public bool IsLogEnabled(LogLevel logLevel) => true;

            public void Log(LogLevel logLevel, Func<string> messageFunc, Exception exception)
            {
                if (logLevel == LogLevel.Error || logLevel == LogLevel.Fatal)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (logLevel == LogLevel.Debug)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                    Console.ForegroundColor = ConsoleColor.White;
                if (exception == null)
                    Console.WriteLine($"{_type.Name} - {logLevel} - {messageFunc()}");
                else
                    Console.WriteLine($"{_type.Name} - {logLevel} - {messageFunc()}: {exception.Message}");
                Console.ResetColor();
            }
        }

        public ILogger CreateLogger(Type type) => new ConsoleLogger(type);
    }
}
