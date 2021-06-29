using Common.ShadowCopy.Logic.Helpers;
using Common.Utils.Standard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace Common.ShadowCopy.Logic
{
    public class SiteWatcher : IDisposable
    {
        private readonly static SiteWatcherConfigSection _config;
        private static List<SiteWatcher> _watchers;

        static SiteWatcher()
        {
            _config = ConfigurationManager.GetSection("siteWatcherConfig") as SiteWatcherConfigSection;
            Config.Init((key) =>
            {
                return ConfigurationManager.AppSettings[key];
            }, new AppLogger());
        }

        public static void Start()
        {
            Logger.Info("SiteWatcher start");
            Clean();
            _watchers = new List<SiteWatcher>();

            foreach (var watcher in _config.Watchers)
            {
                _watchers.Add(new SiteWatcher(watcher as WatcherElement));
            }
        }

        private static void Clean()
        {
            if (_watchers != null)
            {
                foreach (var watcer in _watchers)
                {
                    watcer.Dispose();
                }
            }
        }

        public static void Stop()
        {
            Logger.Info("SiteWatcher stop");
            Clean();
        }

        // non static 

        private WatcherElement _watcherElement;
        private System.Timers.Timer _timer;
        private FileSystemWatcher _filesWatcher;
        private readonly object _isTimerActiveLockObj = new object();
        private readonly object _changedLockObj = new object();
        private readonly object _webpackChangedLockObj = new object();
        public bool IsTimerActive { get; set; }
        public bool IsChanged { get; set; }
        public bool IsWebpackChanged { get; set; }
        public DateTime ExecuteTime { get; set; }

        public SiteWatcher(WatcherElement watcherElement)
        {
            try
            {
                _watcherElement = watcherElement;

                // set file watcher
                _filesWatcher = new FileSystemWatcher();
                if (!string.IsNullOrWhiteSpace(_watcherElement.Filter))
                    _filesWatcher.Filter = _watcherElement.Filter;

                _filesWatcher.Path = _watcherElement.Source;
                _filesWatcher.Deleted += _filesWatcher_Event;
                _filesWatcher.Changed += _filesWatcher_Event;
                _filesWatcher.Created += _filesWatcher_Event;
                _filesWatcher.Renamed += _filesWatcher_Event;
                _filesWatcher.EnableRaisingEvents = true;
                _filesWatcher.IncludeSubdirectories = true;

                // set timer
                _timer = new System.Timers.Timer();
                _timer.Elapsed += _timer_Elapsed;
                _timer.Interval = 100;
                _timer.Enabled = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_isTimerActiveLockObj)
            {
                if (IsTimerActive)
                    return;

                IsTimerActive = true;
            }

            bool wasChanged = IsChanged;
            bool wasWebpackChanged = IsWebpackChanged;

            try
            {
                if (ExecuteTime > DateTime.Now)
                    return;

                if (IsWebpackChanged)
                {
                    IsWebpackChanged = false;

                    lock (_webpackChangedLockObj)
                    {
                        using (var webpackDev = new WebpackProcess(_watcherElement.WebpackPath))
                        {
                            using (var webpackProd = new WebpackProcess(_watcherElement.WebpackPath, "webpack.config.prod.js"))
                            {
                                Parallel.ForEach(new WebpackProcess[] { webpackDev, webpackProd }, (webpack) =>
                                {
                                    try
                                    {
                                        webpack.Start();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex, $"error webpack {webpackDev.ConfigFile}");
                                    }
                                });
                            }
                        }
                    }
                }

                if (IsChanged)
                {
                    IsChanged = false;

                    lock (_changedLockObj)
                    {
                        Logger.Info("Start Sync: " + _watcherElement.Name);
                        string currentFolder = IISHelper.GetApplicationFolder(_watcherElement.Site, _watcherElement.ApplicationPath);
                        Logger.Info($"App path is {currentFolder}");
                        string nextFolder = _watcherElement.Folder1;
                        if (currentFolder.ToLower().Trim('\\') == _watcherElement.Folder1.ToLower().Trim('\\'))
                            nextFolder = _watcherElement.Folder2;

                        Logger.Info($"Delete files from {nextFolder}");
                        FileSystemHelper.DeleteFiles(nextFolder);

                        string syncOutput = null;

                        if (!string.IsNullOrWhiteSpace(_watcherElement.SourceProj) && !string.IsNullOrWhiteSpace(_watcherElement.Framework))
                        {
                            Logger.Info($"Publish files start");
                            var publishRes = DotNetHelper.PublishFiles(_watcherElement.Source, nextFolder, _watcherElement.Framework, _watcherElement.PublishArgs);
                            syncOutput = publishRes.Output;
                            if (!publishRes.IsSuccess)
                            {
                                Logger.Info(syncOutput);
                                return;
                            }

                            Logger.Info("Publish success");

                            if (!string.IsNullOrWhiteSpace(_watcherElement.WebpackPath))
                            {
                                FileSystemHelper.CopyFiles(
                                    Path.Combine(_watcherElement.Source, "wwwroot", "webpack"),
                                    Path.Combine(nextFolder, "wwwroot", "webpack")
                                );
                            }

                            if (!string.IsNullOrWhiteSpace(_watcherElement.CurrentFolder))
                            {
                                FileSystemHelper.EnsureDirectoryExist(_watcherElement.CurrentFolder);
                                FileSystemHelper.DeleteFiles(_watcherElement.CurrentFolder);
                                FileSystemHelper.CopyFiles(nextFolder, _watcherElement.CurrentFolder);
                            };

                            Logger.Info($"Publish files end");
                        }
                        else
                        {
                            Logger.Info($"Copy files start");
                            FileSystemHelper.CopyFiles(_watcherElement.Source, nextFolder);
                            Logger.Info($"Copy files end");
                        }

                        if (Directory.GetFileSystemEntries(nextFolder).Length == 0)
                        {
                            if (!string.IsNullOrWhiteSpace(syncOutput))
                                Logger.Info(syncOutput);

                            Logger.Info("failed to sync application");
                        }
                        else
                        {
                            IISHelper.SetApplicationFolder(nextFolder, _watcherElement.Site, _watcherElement.ApplicationPath);
                            Logger.Info($"App path changed to {nextFolder}");
                        }

                        Logger.Info("End Sync: " + _watcherElement.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ExecuteTime = DateTime.Now.AddSeconds(_watcherElement.WaitBeforeUpdate);
                IsChanged = wasChanged;
                IsWebpackChanged = wasWebpackChanged;
            }
            finally
            {
                lock (_isTimerActiveLockObj)
                {
                    IsTimerActive = false;
                }
            }
        }

        private void _filesWatcher_Event(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (_watcherElement.IgnoresArr.Any(i => Regex.IsMatch(e.Name, i, RegexOptions.IgnoreCase)))
                    return;

                //Logger.Info($"changed file: " + e.FullPath);
                if (_watcherElement.CopyOnlysArr.Any(i => Regex.IsMatch(e.Name, i, RegexOptions.IgnoreCase)))
                {
                    lock (_changedLockObj)
                    {
                        SyncChange(e);
                    }
                }
                else
                {
                    ExecuteTime = DateTime.Now.AddSeconds(_watcherElement.WaitBeforeUpdate);
                    if (!string.IsNullOrWhiteSpace(_watcherElement.WebpackPath) && e.FullPath.StartsWith(_watcherElement.WebpackPath, StringComparison.OrdinalIgnoreCase))
                        IsWebpackChanged = true;
                    else
                        IsChanged = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void SyncChange(FileSystemEventArgs e, string currentFolder = null)
        {
            bool isCurrentFolderNull = false;
            if (currentFolder == null)
            {
                currentFolder = IISHelper.GetApplicationFolder(_watcherElement.Site, _watcherElement.ApplicationPath);
                isCurrentFolderNull = true;
            }

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                case WatcherChangeTypes.Changed:
                    {
                        if (File.Exists(e.FullPath))
                        {
                            string targetFile = Path.Combine(currentFolder, e.Name);
                            FileSystemHelper.EnsureDirectoryExistForFile(targetFile);
                            FileSystemHelper.CopyLocked(Path.Combine(_watcherElement.Source, e.Name), targetFile);
                        }
                    }
                    break;
                case WatcherChangeTypes.Deleted:
                    {
                        string path = Path.Combine(currentFolder, e.Name);
                        if (File.Exists(path))
                            FileSystemHelper.DeleteLocked(new FileInfo(path));
                        else if (Directory.Exists(path))
                            FileSystemHelper.DeleteLocked(new DirectoryInfo(path));
                    }
                    break;
                case WatcherChangeTypes.Renamed:
                    {
                        var er = e as RenamedEventArgs;
                        var oldPath = Path.Combine(currentFolder, er.OldName);
                        if (File.Exists(oldPath))
                            FileSystemHelper.DeleteLocked(new FileInfo(oldPath));

                        if (File.Exists(e.FullPath))
                        {
                            string targetFile = Path.Combine(currentFolder, e.Name);
                            FileSystemHelper.EnsureDirectoryExistForFile(targetFile);
                            FileSystemHelper.CopyLocked(Path.Combine(_watcherElement.Source, e.Name), targetFile);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (isCurrentFolderNull && !string.IsNullOrWhiteSpace(_watcherElement.CurrentFolder))
                SyncChange(e, _watcherElement.CurrentFolder);
        }

        public void Dispose()
        {
            try
            {
                _filesWatcher.Dispose();
            }
            catch { }

            try
            {
                _timer.Dispose();
            }
            catch { }
        }
    }
}
