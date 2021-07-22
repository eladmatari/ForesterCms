using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.ImageMagick
{
    public class ImageMagickMiddlewareOptions
    {
        internal Dictionary<string, ImageMagickProcess> ProcessesDict { get; private set; } = new Dictionary<string, ImageMagickProcess>();
        private readonly static Dictionary<string, object> _watchPathsDict = new Dictionary<string, object>();

        public void AddProcess(ImageMagickProcess process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            if (process.FileProvider == null)
                process.FileProvider = DiHelper.Environment.WebRootFileProvider;

            ProcessesDict[(process.Key ?? "").ToLowerInvariant()] = process;
        }
    }
}
