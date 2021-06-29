using Common.ShadowCopy.Logic;
using Common.ShadowCopy.Logic.Helpers;
using Common.Utils.Standard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ShadowCopy.ConsoleNs
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.Init((key) =>
            {
                return ConfigurationManager.AppSettings[key];
            }, new AppLogger());
            SiteWatcher.Start();
            Console.ReadKey();
            SiteWatcher.Stop();
        }
    }
}
