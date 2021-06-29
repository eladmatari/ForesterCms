using Common.Utils.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ShadowCopy.Logic.Helpers
{
    public class AppLogger : IAppLogger
    {
        public void Error(Exception ex)
        {
            Logger.Error(ex);
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            Logger.Error(ex, message, args);
        }

        public void Info(string message, params object[] args)
        {
            Logger.Info(message, args);
        }
    }
}
