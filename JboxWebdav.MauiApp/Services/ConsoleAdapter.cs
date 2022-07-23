using System;
using NWebDav.Server.Logging;

namespace JboxWebdav.MauiApp.Services
{
    public class Log4NetAdapter : ILoggerFactory
    {
        private class Logger : ILogger
        {
            private readonly Type _type;

            public Logger(Type type)
            {
                _type = type;
            }

            public bool IsLogEnabled(LogLevel logLevel) => true;

            public void Log(LogLevel logLevel, Func<string> messageFunc, Exception exception)
            {
                if (exception == null)
                {
                    
                }
                else
                {
                    
                }
            }
        }

        public ILogger CreateLogger(Type type) => new Logger(type);
    }
}
