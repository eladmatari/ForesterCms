using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.ImageMagick
{
    public static class ImageMagickMiddlewareExtensions
    {
        public static IApplicationBuilder UseImageMagick(this IApplicationBuilder app, Action<ImageMagickMiddlewareOptions> setOptionsAction)
        {
            var middlewareOptions = new ImageMagickMiddlewareOptions();
            setOptionsAction?.Invoke(middlewareOptions);

            ImageMagickMiddleware.Options = middlewareOptions;
            ImageMagickMiddleware.Watcher = new ImageMagickMiddlewareWatcher();
            app.UseMiddleware<ImageMagickMiddleware>();

            return app;
        }
    }
}
