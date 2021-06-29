using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common.Utils.Standard
{
    public static class FileSystemHelper
    {
        public const int WAIT_AFTER_FAILURE = 100;
        public const int MAX_COUNTER = 100;

        private static readonly Dictionary<char, string> _invalidCharReplaceDict = new Dictionary<char, string>();
        private static readonly Dictionary<string, char> _invalidCharRestoreDict = new Dictionary<string, char>();

        static FileSystemHelper()
        {
            int counter = 0;

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                string replacement = $"~{counter}~";

                _invalidCharReplaceDict[c] = replacement;
                _invalidCharRestoreDict[replacement] = c;

                counter++;
            }
        }

        public static void EnsureDirectoryExistForFile(string filePath)
        {
            var pathArr = filePath.Split('\\');
            var dirPath = string.Join("\\", pathArr.Take(pathArr.Length - 1));

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }

        public static string GetSafeFileName(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            var sb = new StringBuilder();

            foreach (char c in s)
            {
                string replacement;
                if (c == '~')
                    sb.Append('_');
                else if (_invalidCharReplaceDict.TryGetValue(c, out replacement))
                    sb.Append(replacement);
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        public static string RestoreUnsafeFileName(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            try
            {
                var sb = new StringBuilder();

                for (int i = 0; i < s.Length; i++)
                {
                    var c = s[i];
                    if (c == '~')
                    {
                        int replacementNumber = 0;

                        for (int j = i + 1; j < s.Length; j++)
                        {
                            c = s[j];
                            if (c == '~')
                                break;

                            replacementNumber *= 10;
                            replacementNumber += int.Parse(s[j].ToString());
                        }

                        string replacement = $"~{replacementNumber}~";

                        sb.Append(_invalidCharRestoreDict[replacement]);
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void EnsureDirectoryExist(string dirPath)
        {
            if (string.IsNullOrWhiteSpace(dirPath))
                return;

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }

        public static void RemoveReadOnly(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            var attributes = File.GetAttributes(filePath);

            if (attributes.HasFlag(FileAttributes.ReadOnly))
                File.SetAttributes(filePath, attributes & ~FileAttributes.ReadOnly);
        }

        public static void DeleteLocked(this FileInfo file, int maxCounter = MAX_COUNTER, int waitAfterFailure = WAIT_AFTER_FAILURE)
        {
            int counter = 0;
            RemoveReadOnly(file.FullName);

            while (true)
            {
                try
                {
                    file.Delete();
                    break;
                }
                catch (UnauthorizedAccessException)
                {
                    if (counter >= maxCounter)
                        throw;

                    counter++;
                    Thread.Sleep(waitAfterFailure);
                }
            }
        }

        public static void DeleteLocked(this DirectoryInfo dir, int maxCounter = MAX_COUNTER, int waitAfterFailure = WAIT_AFTER_FAILURE)
        {
            int counter = 0;

            while (true)
            {
                try
                {
                    dir.Delete(true);
                    break;
                }
                catch (UnauthorizedAccessException)
                {
                    if (counter >= maxCounter)
                        throw;

                    counter++;
                    Thread.Sleep(waitAfterFailure);
                }
            }
        }

        public static void CopyLocked(string source, string target, int maxCounter = MAX_COUNTER, int waitAfterFailure = WAIT_AFTER_FAILURE)
        {
            RemoveReadOnly(target);
            int counter = 0;

            while (true)
            {
                try
                {
                    File.Copy(source, target, true);
                    break;
                }
                catch (UnauthorizedAccessException)
                {
                    if (counter >= maxCounter)
                        throw;

                    counter++;
                    Thread.Sleep(waitAfterFailure);
                }
            }
        }

        public static void DeleteFiles(string path)
        {
            var di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.DeleteLocked();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.DeleteLocked();
            }
        }

        public static void CopyFiles(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                CopyLocked(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in Directory.GetDirectories(sourceDir))
                CopyFiles(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }
    }
}
