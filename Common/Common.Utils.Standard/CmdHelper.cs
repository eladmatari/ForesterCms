using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Common.Utils.Standard
{
    public static class CmdHelper
    {
        public static Process Run(string cmd, string startDir)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {cmd}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = startDir
            };

            process.OutputDataReceived += (sender, args) => { };
            process.Start();

            return process;

        }
    }
}
