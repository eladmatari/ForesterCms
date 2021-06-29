using Common.Utils.Standard;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils.Logging
{
    public class AppLogger : IAppLogger, ILogger, IDisposable
    {
        public void Error(Exception ex)
        {
            Logger.Error(ex);
            DbLogger.AddLog(ex, null, null, null, null, null);
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            Logger.Error(ex, message, args);
            DbLogger.AddLog(ex, message, null, null, null, null);
        }

        public void Info(string message, params object[] args)
        {
            Logger.Info(message, args);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Logger.Error(exception, JsonHelper.Serialize(new
            {
                logLevel,
                eventId,
                state
            }));
        }

        public void Dispose()
        {

        }
    }
}
