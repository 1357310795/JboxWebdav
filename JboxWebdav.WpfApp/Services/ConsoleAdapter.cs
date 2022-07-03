using System;
using log4net;
using NWebDav.Server.Logging;

namespace JboxWebdav.WpfApp.Services
{
    public class Log4NetAdapter : ILoggerFactory
    {
        private class Logger : ILogger
        {
            private readonly Type _type;
            private ILog s_log;

            public Logger(Type type)
            {
                _type = type;
                s_log = LogManager.GetLogger(_type);
            }

            public bool IsLogEnabled(LogLevel logLevel) => true;

            public void Log(LogLevel logLevel, Func<string> messageFunc, Exception exception)
            {
                if (exception == null)
                {
                    switch (logLevel)
                    {
                        case LogLevel.Debug:
                            s_log.Debug(messageFunc());
                            break;
                        case LogLevel.Fatal:
                            s_log.Fatal(messageFunc());
                            break;
                        case LogLevel.Warning:
                            s_log.Warn(messageFunc());
                            break;
                        case LogLevel.Error:
                            s_log.Error(messageFunc());
                            break;
                        case LogLevel.Info:
                            s_log.Info(messageFunc());
                            break;
                    }
                }
                else
                {
                    switch (logLevel)
                    {
                        case LogLevel.Debug:
                            s_log.Debug(messageFunc(), exception);
                            break;
                        case LogLevel.Fatal:
                            s_log.Fatal(messageFunc(), exception);
                            break;
                        case LogLevel.Warning:
                            s_log.Warn(messageFunc(), exception);
                            break;
                        case LogLevel.Error:
                            s_log.Error(messageFunc(), exception);
                            break;
                        case LogLevel.Info:
                            s_log.Info(messageFunc(), exception);
                            break;
                    }
                }
            }
        }

        public ILogger CreateLogger(Type type) => new Logger(type);
    }
}
