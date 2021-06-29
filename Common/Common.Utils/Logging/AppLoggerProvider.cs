using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils.Logging
{
    public class AppLoggerProvider : ILoggerProvider
    {
        public AppLogger AppLogger { get; set; } = new AppLogger();

        public ILogger CreateLogger(string categoryName)
        {
            return AppLogger;
        }

        public void Dispose()
        {

        }
    }
}
