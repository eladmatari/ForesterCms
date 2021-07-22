using Common.Utils;
using Common.Utils.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.ImageMagick
{
    public class ImageMagickMiddlewareWatcher : IDisposable
    {
        public ImageMagickMiddlewareWatcher()
        {
            string path = Path.Combine(DiHelper.Environment.WebRootPath, ImageMagickMiddleware.CacheFolder);
            FileSystemHelper.EnsureDirectoryExist(path);

            _filesWatcher = new FileSystemWatcher();
            _filesWatcher.Path = path;
            _filesWatcher.IncludeSubdirectories = true;
            _filesWatcher.Deleted += _filesWatcher_Deleted;


            _filesWatcher.EnableRaisingEvents = true;
        }

        private void _filesWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            ImageMagickMiddleware.DeleteMemoryCacheKeys();
        }

        private FileSystemWatcher _filesWatcher;

        public void Dispose()
        {
            try
            {
                _filesWatcher.EnableRaisingEvents = false;
                _filesWatcher.Dispose();
            }
            catch { }
        }
    }
}
