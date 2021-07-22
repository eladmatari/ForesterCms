using Common.Utils.Standard;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.ImageMagick
{
    public class ImageMagickProcess
    {
        private string _key;
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = FileSystemHelper.GetSafeFileName(value);
            }
        }
        public int Width { get; set; }
        public int Height { get; set; }
        public ImageMagickMode Mode { get; set; }
        public IFileProvider FileProvider { get; set; }

        private string _requestPath = "";
        public string RequestPath
        {
            get
            {
                return _requestPath;
            }
            set
            {
                _requestPath = value?.Trim('/').ToLower() ?? "";
                _requestPathFileKey = FileSystemHelper.GetSafeFileName(_requestPath);
            }
        }

        private string _requestPathFileKey;


        public string GetUniqueKey()
        {
            string uniqueKey = $"{Key}_{Width}_{Height}_{(int)Mode}_{_requestPathFileKey}";

            return uniqueKey;
        }
    }
}
