using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common.Utils.Standard
{
    public static class IISHelper
    {
        public const int WAIT_AFTER_FAILURE = 100;
        public const int MAX_COUNTER = 100;

        public static void SetApplicationFolder(string folder, string siteName, string applicationPath = null)
        {
            var serverManager = new ServerManager();

            var site = serverManager.Sites.FirstOrDefault(i => i.Name == siteName);
            Application app = null;

            if (!string.IsNullOrWhiteSpace(applicationPath))
                app = site.Applications.FirstOrDefault(i => i.Path == applicationPath);
            else
                app = site.Applications.FirstOrDefault(i => i.Path == "/");

            if (app.VirtualDirectories[0].PhysicalPath != folder)
            {
                app.VirtualDirectories[0].PhysicalPath = folder;
                serverManager.CommitChanges();
            }
        }

        public static string GetApplicationFolder(string siteName, string applicationPath = null)
        {
            var serverManager = new ServerManager();

            var site = serverManager.Sites.FirstOrDefault(i => i.Name == siteName);
            Application app = null;

            if (!string.IsNullOrWhiteSpace(applicationPath))
                app = site.Applications.FirstOrDefault(i => i.Path == applicationPath);
            else
                app = site.Applications.FirstOrDefault(i => i.Path == "/");

            return app.VirtualDirectories[0].PhysicalPath;
        }

        public static bool StartAppPool(string name, int counter = 0, int maxCounter = MAX_COUNTER, int waitAfterFailure = WAIT_AFTER_FAILURE)
        {
            try
            {
                var serverManager = new ServerManager();

                foreach (var appPool in serverManager.ApplicationPools)
                {
                    if (appPool.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        switch (appPool.State)
                        {
                            case ObjectState.Stopping:
                            case ObjectState.Stopped:
                            case ObjectState.Unknown:
                                appPool.Start();
                                return true;
                            default:
                                return true;
                        }

                    }
                }
            }
            catch (Exception)
            {
                if (counter >= maxCounter)
                    throw;

                Thread.Sleep(waitAfterFailure);
                StartAppPool(name, counter + 1, maxCounter, waitAfterFailure);
            }

            return false;
        }

        public static bool StopAppPool(string name, int counter = 0, int maxCounter = MAX_COUNTER, int waitAfterFailure = WAIT_AFTER_FAILURE)
        {
            try
            {
                var serverManager = new ServerManager();

                foreach (var appPool in serverManager.ApplicationPools)
                {
                    if (appPool.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        switch (appPool.State)
                        {
                            case ObjectState.Starting:
                            case ObjectState.Started:
                            case ObjectState.Unknown:
                                appPool.Stop();
                                return true;
                            default:
                                return true;
                        }

                    }
                }
            }
            catch (Exception)
            {
                if (counter >= maxCounter)
                    throw;

                Thread.Sleep(waitAfterFailure);
                StartAppPool(name, counter + 1, maxCounter, waitAfterFailure);
            }

            return false;
        }
    }
}
