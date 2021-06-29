using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ShadowCopy.Logic.Helpers
{
    public static class Logger
    {
        private static readonly NLog.Logger _logger;
        static Logger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }


        /// <summary>
        /// Highest level: important stuff down
        /// </summary>
        public static void Fatal(Exception ex, string message, params object[] args)
        {
            _logger.Fatal(ex, message, args);
        }

        /// <summary>
        /// Highest level: important stuff down
        /// </summary>
        public static void Fatal(string message, params object[] args)
        {
            _logger.Fatal(message, args);
        }

        /// <summary>
        /// application crashes / exceptions.
        /// </summary>
        public static void Error(Exception ex)
        {
            _logger.Error(ex);
        }

        /// <summary>
        /// application crashes / exceptions.
        /// </summary>
        public static void Error(Exception ex, string message, params object[] args)
        {
            _logger.Error(ex, message, args);
        }

        /// <summary>
        /// application crashes / exceptions.
        /// </summary>
        public static void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        /// <summary>
        /// Incorrect behavior but the application can continue
        /// </summary>
        public static void Warn(Exception ex, string message, params object[] args)
        {
            _logger.Warn(ex, message, args);
        }

        /// <summary>
        /// Incorrect behavior but the application can continue
        /// </summary>
        public static void Warn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        /// <summary>
        /// Normal behavior like mail sent, user updated profile etc.
        /// </summary>
        public static void Info(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        /// <summary>
        /// Executed queries, user authenticated, session expired
        /// </summary>
        public static void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        /// <summary>
        /// Begin method X, end method X etc
        /// </summary>
        public static void Trace(string message, params object[] args)
        {
            _logger.Trace(message, args);
        }
    }
}
