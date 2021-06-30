using Common.Utils;
using Common.Utils.Logging;
using Common.Utils.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Resources
{
    public class WebpackHelper : IDisposable
    {
        public static string SourceDirectory
        {
            get
            {
                return Path.Combine(DiHelper.Environment.ContentRootPath, "ClientApp");
            }
        }

        public static string TargetDirectory
        {
            get
            {
                return Path.Combine(DiHelper.Environment.ContentRootPath, "wwwroot", "webpack");
            }
        }

        static WebpackHelper()
        {
            Instance = new WebpackHelper();
        }

        public static WebpackHelper Instance { get; private set; }

        private WebpackHelper()
        {
            SetVersionData();
            _webpack = new WebpackProcess(SourceDirectory);
            _webpackProd = new WebpackProcess(SourceDirectory, "webpack.config.prod.js");
        }

        private void _filesWatcher_Event(object sender, FileSystemEventArgs e)
        {
            SetVersionData();
        }

        private void SetVersionData()
        {
            lock (_filesWatcherLockObj)
            {
                VersionUpdateDate = DateTime.Now;
                VersionUniqueId = CryptHelper.GetRandomStringAlphaNumeric(20);
            }
        }

        private WebpackProcess _webpack;
        private WebpackProcess _webpackProd;
        private FileSystemWatcher _filesWatcher;
        private readonly object _filesWatcherLockObj = new object();
        public DateTime VersionUpdateDate { get; private set; }
        public string VersionUniqueId { get; private set; }

        public void Start()
        {
            Logger.Info("Webpack start");
            _webpack.Start();
            _webpackProd.Start();
        }

        public void StartWatch()
        {
            _webpack.StartWatch();
            _webpackProd.StartWatch();
        }

        public void StartWatchVersion()
        {
            StopWatchVersion();

            _filesWatcher = new FileSystemWatcher();

            _filesWatcher.Path = TargetDirectory;
            _filesWatcher.Deleted += _filesWatcher_Event;
            _filesWatcher.Changed += _filesWatcher_Event;
            _filesWatcher.Created += _filesWatcher_Event;
            _filesWatcher.Renamed += _filesWatcher_Event;
            _filesWatcher.EnableRaisingEvents = true;
            _filesWatcher.IncludeSubdirectories = true;
        }

        public void StopWatchVersion()
        {
            try
            {
                _filesWatcher?.Dispose();
            }
            catch { }
        }

        public void Stop()
        {
            _webpack.Stop();
            _webpackProd.Stop();
            Logger.Info("Webpack stop");
        }

        public void Dispose()
        {
            Stop();
            StopWatchVersion();
        }
    }
}
