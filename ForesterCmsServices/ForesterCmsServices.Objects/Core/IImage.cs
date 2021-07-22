using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Objects.Core
{
    public interface IImage
    {
        int? Id { get; set; }
        string Alt { get; set; }

        string GetImageUrl();
        string GetImageMobileUrl();
        string GetImageMobileUrl(bool isFullUrl);
        string GetImageMobileUrl(bool isDefaultMobile, bool isFullUrl);
        string GetImageUrl(bool isFullUrl);
        string GetImageUrl(bool isMobile, bool isMobileDefault, bool isFullUrl);
        bool HasMobile { get; }
    }
}
