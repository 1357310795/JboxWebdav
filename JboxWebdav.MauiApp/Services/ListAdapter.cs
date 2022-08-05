using System;
using NWebDav.Server.Logging;

namespace JboxWebdav.MauiApp.Services
{
    public class ListAdapter : ILoggerFactory
    {
        private class Logger : ILogger
        {
            private readonly Type _type;

            public Logger(Type type)
            {
                _type = type;
                if (LogStorage.logs == null)
                    LogStorage.logs = new List<string>(128);
            }

            public bool IsLogEnabled(LogLevel logLevel) => true;

            public void Log(LogLevel logLevel, Func<string> messageFunc, Exception exception)
            {
                if (exception == null)
                    LogStorage.logs.Add($"{_type.Name} - {logLevel} - {messageFunc()}");
                else
                    LogStorage.logs.Add($"{_type.Name} - {logLevel} - {messageFunc()}: {exception.Message}");
            }
        }

        public ILogger CreateLogger(Type type) => new Logger(type);
    }

    public static class LogStorage
    {
        public static List<string> logs;
    }
}
