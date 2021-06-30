using Common.Utils;
using Common.Utils.Logging;
using Common.Utils.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Resources
{
    public abstract class ResourceGroupBase : IDisposable
    {
        protected abstract string FilesPath { get; }
        protected abstract string OutputFileExstension { get; }

        private object _lockObjsDictLockObj = new object();
        private Dictionary<string, object> _lockObjsDict = new Dictionary<string, object>();
        private Dictionary<string, ResourceGroupData> _filesDict = new Dictionary<string, ResourceGroupData>();
        private FileSystemWatcher _filesWatcher;

        public void StartWatch()
        {
            try
            {
                Logger.Info($"start watcher {FilesPath}");
                DisposeWatcher();
                DeleteFiles();

                _filesWatcher = new FileSystemWatcher();
                _filesWatcher.Path = Path.Combine(DiHelper.Environment.WebRootPath, FilesPath);
                _filesWatcher.Deleted += _filesWatcher_Event;
                _filesWatcher.Changed += _filesWatcher_Event;
                _filesWatcher.Created += _filesWatcher_Event;
                _filesWatcher.Renamed += _filesWatcher_Event;
                _filesWatcher.EnableRaisingEvents = true;
                _filesWatcher.IncludeSubdirectories = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void StopWatch()
        {
            Logger.Info($"stop watcher {FilesPath}");
            DisposeWatcher();
        }

        public void DisposeWatcher()
        {
            if (_filesWatcher == null)
                return;

            Logger.Info($"dispose watcher static {OutputFileExstension}");
            try
            {
                _filesWatcher.EnableRaisingEvents = false;
                _filesWatcher.Dispose();
            }
            finally
            {
                _filesWatcher = null;
            }
        }

        public void Dispose()
        {

            Logger.Info($"dispose class {OutputFileExstension}");
            DisposeWatcher();
        }

        private void DeleteFiles()
        {
            Logger.Info($"delete files static {OutputFileExstension}");
            var dir = new DirectoryInfo(Path.Combine(DiHelper.Environment.WebRootPath, "static"));
            if (!dir.Exists)
                return;

            foreach (var file in dir.GetFiles())
            {
                if (OutputFileExstension.Equals(file.Extension, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var key = file.Name.Substring(0, file.Name.Length - OutputFileExstension.Length).ToLower();
                        if (key.EndsWith(".min", StringComparison.OrdinalIgnoreCase))
                            key = key.Substring(0, key.Length - 4);

                        if (!_filesDict.ContainsKey(key))
                            return;

                        lock (GetLockObj(key))
                        {
                            FileSystemHelper.DeleteLocked(file);
                            _filesDict.Remove(key);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
        }

        private void _filesWatcher_Event(object sender, FileSystemEventArgs e)
        {
            try
            {
                string fileName = ProcessFileName(e.Name);
                var keysToRemove = _filesDict.Where(i => i.Value.Files.Any(j => j.Equals(fileName, StringComparison.OrdinalIgnoreCase))).Select(i => i.Key).ToList();
                foreach (var key in keysToRemove)
                {
                    lock (GetLockObj(key))
                    {
                        _filesDict.Remove(key);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        internal static string ProcessFileName(string fileName)
        {
            return fileName.Trim(' ', '/', '\\').ToLower().Replace('\\', '/');
        }

        internal static void ProcessFileNames(string[] fileNames)
        {
            for (int i = 0; i < fileNames.Length; i++)
            {
                fileNames[i] = ProcessFileName(fileNames[i]);
            }
        }

        private object GetLockObj(string key)
        {
            object lockObj;
            if (_lockObjsDict.TryGetValue(key, out lockObj))
                return lockObj;

            lock (_lockObjsDictLockObj)
            {
                if (_lockObjsDict.TryGetValue(key, out lockObj))
                    return lockObj;

                lockObj = _lockObjsDict[key] = new object();
                return lockObj;
            }
        }

        public string GetOrAddBundle(string key, params string[] filesPaths)
        {
            key = key.ToLower();
            ProcessFileNames(filesPaths);
            ResourceGroupData resourceData;
            if (!_filesDict.TryGetValue(key, out resourceData) || !resourceData.EqualsFiles(filesPaths))
            {
                lock (GetLockObj(key))
                {
                    if (!_filesDict.TryGetValue(key, out resourceData) || !resourceData.EqualsFiles(filesPaths))
                    {
                        resourceData = new ResourceGroupData();
                        AddBundle(key, filesPaths);
                        resourceData.Key = key;
                        resourceData.Files = filesPaths;
                        _filesDict[key] = resourceData;
                    }
                }
            }

            string minStr = ResourceGroupHelper.IsDebugModeEnabled == true ? "" : ".min";
            return $"static/{key}{minStr}{OutputFileExstension}?v={resourceData.UniqueId}";
        }

        private void AddBundle(string key, string[] filesPaths)
        {
            string basePath = Path.Combine(DiHelper.Environment.WebRootPath, FilesPath);
            string newFileBase = Path.Combine(DiHelper.Environment.WebRootPath, "static", key) + $"_{Config.CreateDate.Ticks}";

            var sb = new StringBuilder();

            foreach (var filePath in filesPaths)
            {
                string fileFullPath = Path.Combine(basePath, filePath);
                if (!File.Exists(fileFullPath))
                    continue;

                sb.AppendLine($"/*file: {filePath}*/");
                if (Config.Environment == EnvironmentType.Prod || filePath.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine(File.ReadAllText(fileFullPath, Encoding.UTF8));
                }
                else
                {
                    var lines = File.ReadAllLines(fileFullPath, Encoding.UTF8);
                    int remarkOpens = 0;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];

                        int remarkOpensBefore = remarkOpens;
                        remarkOpens += Regex.Matches(line, "/\\*").Count;
                        remarkOpens -= Regex.Matches(line, "\\*/").Count;

                        if (remarkOpens < 0)
                            remarkOpens = 0;

                        if (i % 10 == 0)
                        {
                            if ((remarkOpens == 0 && remarkOpensBefore == 0) || (remarkOpensBefore == 0 && remarkOpens > 0))
                                sb.AppendLine($"/*file: {filePath}, {i + 1}*/");
                            else
                                sb.AppendLine($"file: {filePath}, {i + 1}");
                        }

                        sb.AppendLine(line);
                    }
                }
            }

            string fileText = sb.ToString();
            fileText = Process(fileText);
            FileSystemHelper.EnsureDirectoryExistForFile(newFileBase);

            string newFilePath = newFileBase + OutputFileExstension;
            string newFileMinPath = newFileBase + ".min" + OutputFileExstension;

            WriteFiles(ref fileText, newFilePath, newFileMinPath, key + OutputFileExstension);
        }

        protected virtual void WriteFiles(ref string fileText, string newFilePath, string newFileMinPath, string fileName)
        {

        }

        protected virtual string Process(string s)
        {
            return s;
        }
    }
}
