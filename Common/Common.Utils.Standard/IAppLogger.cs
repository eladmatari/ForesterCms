using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Utils.Standard
{
    public interface IAppLogger
    {
        void Error(Exception ex);
        void Error(Exception ex, string message, params object[] args);
        void Info(string message, params object[] args);
    }
}
