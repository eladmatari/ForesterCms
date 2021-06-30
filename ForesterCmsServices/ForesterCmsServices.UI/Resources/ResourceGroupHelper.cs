using Common.Utils;
using Common.Utils.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Resources
{
    public static class ResourceGroupHelper
    {
        private static string[] _cleanFilesExtensions = new string[] { ".js", ".css", ".map", ".scss" };

        static ResourceGroupHelper()
        {
            CleanStatic();

            Scripts = new ResourceGroupScripts();
            Css = new ResourceGroupCss();
        }

        private static void CleanStatic()
        {
            Logger.Debug($"{nameof(ResourceGroupHelper)} {nameof(CleanStatic)} Start");
            var dir = new DirectoryInfo(Path.Combine(DiHelper.Environment.WebRootPath, "static"));
            if (!dir.Exists)
                return;

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                if (!_cleanFilesExtensions.Contains(file.Extension))
                    continue;

                try
                {
                    Logger.Debug($"{nameof(ResourceGroupHelper)} {nameof(CleanStatic)} Delete - {file.FullName}");
                    file.Delete();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        public static ResourceGroupScripts Scripts { get; private set; }

        public static ResourceGroupCss Css { get; private set; }

        public static void SetDebugMode(bool isDebug)
        {
            HttpContextHelper.Current.Session.SetData("IsResourceDebug", isDebug);
        }

        public static bool? IsDebugModeEnabled
        {
            get
            {
                return HttpContextHelper.Current.Session.GetData<bool?>("IsResourceDebug");
            }
        }
    }
}
