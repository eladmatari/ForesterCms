using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public interface IAppLogger
    {
        void Error(Exception ex);
        void Error(Exception ex, string message, params object[] args);
        void Info(string message, params object[] args);
    }
}
