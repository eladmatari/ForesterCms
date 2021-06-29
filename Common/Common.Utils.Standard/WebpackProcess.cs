using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Utils.Standard
{
    public class WebpackProcess : IDisposable
    {
        private Process _process;
        public string Directory { get; private set; }
        public string ConfigFile { get; private set; }
        public bool IsWatched { get; private set; }


        public WebpackProcess(string directory, string configFile = "webpack.config.js")
        {
            Directory = directory;
            ConfigFile = configFile;
        }

        private object _lockObj = new object();
        private object _watcherLockObj = new object();

        public bool Start()
        {
            KillProcess();

            lock (_lockObj)
            {
                Config.Logger?.Info($"webpack start {ConfigFile}");
                _process = CmdHelper.Run($"webpack --config {ConfigFile} --json", Directory);
                string resultJson = _process.StandardOutput.ReadToEnd();
                _process.WaitForExit();

                var result = JsonHelper.TryDeserialize<WebpackResult>(resultJson);
                Config.Logger?.Info(JsonHelper.Serialize(result));

                if (result?.Errors?.Count > 0)
                {
                    Config.Logger?.Info($"webpack end failure {ConfigFile}");
                    return false;
                }

                Config.Logger?.Info($"webpack end success {ConfigFile}");
                return true;
            }
        }

        public void StartWatch()
        {
            StartWatch(0);
        }

        private void StartWatch(int counter)
        {
            bool wasWatched = IsWatched;
            KillProcess();
            if (counter > 10)
                return;

            lock (_lockObj)
            {
                IsWatched = true;
            }

            if (wasWatched)
                Thread.Sleep(5000);

            Task.Run(() =>
            {
                lock (_watcherLockObj)
                {
                    Config.Logger?.Info($"webpack watch start");

                    try
                    {
                        bool isReady = false;
                        var outputSb = new StringBuilder();
                        _process = CmdHelper.Run($"webpack --config {ConfigFile} --watch", Directory);
                        _process.OutputDataReceived += (sender, args) =>
                        {
                            try
                            {
                                outputSb.AppendLine(args.Data);
                                bool isBuildEnd = args.Data?.IndexOf("watching files for updates", StringComparison.OrdinalIgnoreCase) >= 0;

                                if (isBuildEnd)
                                {
                                    if (!isReady)
                                    {
                                        Config.Logger?.Info($"webpack watch ready");
                                        isReady = true;
                                    }

                                    Config.Logger?.Info(outputSb.ToString());
                                    outputSb.Clear();
                                }
                            }
                            catch (Exception ex)
                            {
                                Config.Logger?.Error(ex);
                            }
                        };
                        _process.BeginOutputReadLine();
                        _process.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            Config.Logger?.Info("webpack watch exception: " + ex.ToString());
                        }
                        catch { }
                    }
                }

                if (IsWatched)
                {
                    counter++;
                    Config.Logger?.Info($"webpack watch retry {counter}");
                    StartWatch(counter);
                }
            });
        }

        public void Stop()
        {
            KillProcess();
        }

        private void KillProcess()
        {
            lock (_lockObj)
            {
                if (_process != null)
                {
                    try
                    {
                        _process.Kill();
                    }
                    catch { }

                    try
                    {
                        _process.Dispose();
                    }
                    catch { }

                    _process = null;
                    IsWatched = false;
                }
            }
        }

        public void Dispose()
        {
            KillProcess();
        }

        private class WebpackResult
        {
            public string Hash { get; set; }
            public string version { get; set; }
            public int Time { get; set; }
            public long BuiltAt { get; set; }
            public List<dynamic> Errors { get; set; }
        }
    }
}
