using Common.Utils;
using Common.Utils.Logging;
using Common.Utils.Standard;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.ImageMagick
{
    public class ImageMagickMiddleware
    {
        private readonly RequestDelegate _next;
        internal readonly static string[] FileExtensions = new string[] { ".jpg", ".jpeg", ".png" };
        internal readonly static string FileExtensionsRegex = string.Join("|", FileExtensions.Select(i => i + "$"));
        internal readonly static string CacheFolder = "static/process";

        internal static ImageMagickMiddlewareOptions Options { get; set; }
        internal static ImageMagickMiddlewareWatcher Watcher { get; set; }

        private readonly static object _targetKeysDictLockObj = new object();
        internal readonly static Dictionary<string, object> _targetKeysDict = new Dictionary<string, object>();
        private readonly static Dictionary<string, object> _targetKeysLockObjs = new Dictionary<string, object>();

        internal static object GetLockObj(string targetKey)
        {
            object obj;
            if (_targetKeysLockObjs.TryGetValue(targetKey, out obj))
                return obj;

            lock (_targetKeysDictLockObj)
            {
                if (_targetKeysLockObjs.TryGetValue(targetKey, out obj))
                    return obj;

                return _targetKeysLockObjs[targetKey] = new object();
            }
        }

        internal static void DeleteMemoryCacheKeys()
        {
            var keys = _targetKeysDict.Keys.ToList();

            foreach (var key in keys)
            {
                lock (GetLockObj(key))
                {
                    _targetKeysDict.Remove(key);
                }
            }
        }

        public ImageMagickMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var process = GetProcess(context);
            if (process == null)
            {
                await _next(context);
                return;
            }

            string targetPath = GetTargetPath(context, process);
            if (targetPath != null)
                context.Request.Path = targetPath;

            await _next(context);
        }

        public string GetTargetPath(HttpContext context, ImageMagickProcess process)
        {
            var provider = process.FileProvider ?? DiHelper.Environment.WebRootFileProvider;

            string path = context.Request.Path.Value.TrimStart('/');
            if (process.RequestPath.Length > 0)
                path = path.Substring(process.RequestPath.Length + 1);

            string targetKey = GetTargetKey(path, process);
            string response = $"/{CacheFolder}/" + targetKey;

            if (_targetKeysDict.ContainsKey(targetKey))
                return response;

            lock (GetLockObj(targetKey))
            {
                if (_targetKeysDict.ContainsKey(targetKey))
                    return response;

                IFileInfo fileInfo = provider.GetFileInfo(path);

                if (!fileInfo.Exists)
                    return null;

                string targetPath = response.Substring(1);
                var targetFileInfo = DiHelper.Environment.WebRootFileProvider.GetFileInfo(targetPath);
                if (!targetFileInfo.Exists || fileInfo.LastModified != targetFileInfo.LastModified)
                {
                    try
                    {
                        switch (process.Mode)
                        {
                            case ImageMagickMode.Resize:
                            case ImageMagickMode.ResizeMax:
                                {
                                    bool ignoreAspectRatio = process.Mode == ImageMagickMode.Resize;

                                    if (!Resize(process, fileInfo, targetFileInfo, ignoreAspectRatio))
                                        return null;
                                }
                                break;
                            default:
                                break;
                        }

                        File.SetLastWriteTimeUtc(targetFileInfo.PhysicalPath, fileInfo.LastModified.DateTime);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        return null;
                    }
                }

                _targetKeysDict[targetKey] = null;
            }

            return response;
        }

        private static bool Resize(ImageMagickProcess process, IFileInfo fileInfo, IFileInfo targetFileInfo, bool ignoreAspectRatio)
        {
            FileSystemHelper.EnsureDirectoryExistForFile(targetFileInfo.PhysicalPath);

            using (MagickImage image = new MagickImage(fileInfo.PhysicalPath))
            {
                int width = process.Width;
                int height = process.Height;

                if ((width <= 0 && height <= 0) || width > image.Width || height > image.Height)
                    return false;

                if (width <= 0)
                {
                    var ratio = height / (decimal)image.Height;
                    width = (int)(image.Width * ratio);
                }
                else if (height <= 0)
                {
                    var ratio = width / (decimal)image.Width;
                    height = (int)(image.Height * ratio);
                }

                var size = new MagickGeometry(width, height);
                size.IgnoreAspectRatio = ignoreAspectRatio;

                image.Resize(size);
                image.Write(targetFileInfo.PhysicalPath);
            }

            return true;
        }

        public string GetTargetKey(string path, ImageMagickProcess process)
        {
            var pathArr = path.Split('.');
            string targetPath = $"{string.Join(".", pathArr.Take(pathArr.Length - 1))}_{process.GetUniqueKey()}.{pathArr.Last()}";

            return targetPath.ToLower();
        }

        internal readonly static char[] _urlPathSplitter = new char[] { '/' };

        internal static ImageMagickProcess GetProcess(HttpContext context)
        {
            if (!context.Request.Query.ContainsKey("process"))
                return null;

            // is not supported image file or is in cache folder
            if (!Regex.IsMatch(context.Request.Path, FileExtensionsRegex, RegexOptions.IgnoreCase))
                return null;

            if (Regex.IsMatch(context.Request.Path, $"^/{CacheFolder}/", RegexOptions.IgnoreCase))
                return null;

            ImageMagickProcess process;
            if (!Options.ProcessesDict.TryGetValue(context.Request.Query["process"][0].ToLower(), out process))
                return null;

            return process;
        }
    }
}
