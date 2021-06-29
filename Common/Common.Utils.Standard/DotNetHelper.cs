using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Common.Utils.Standard
{
    public static class DotNetHelper
    {
        public static PublishResult PublishFiles(string source, string target, string framework, string optionsArgs = null)
        {
            // Start the child process.
            using (var p = new Process())
            {
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "dotnet.exe";
                p.StartInfo.Arguments = $@"publish ""{source}"" -o ""{target}"" {optionsArgs} -v n";
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                return new PublishResult()
                {
                    IsSuccess = output.Contains("0 Error(s)"),
                    Output = output
                };
            }
        }

        public class PublishResult
        {
            public bool IsSuccess { get; set; }
            public string Output { get; set; }
        }
    }
}
