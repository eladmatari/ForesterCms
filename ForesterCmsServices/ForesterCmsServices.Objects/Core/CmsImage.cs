using Common.Utils.Logging;
using Common.Utils.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Objects.Core
{
    public class CmsImage : ICloneable, IImage
    {
        public CmsImage() { }

        public CmsImage(string resources, int? id, string alt)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(resources))
                {
                    Resources = JsonHelper.TryDeserialize<ImageResourcesContainer>(resources);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("error loading image resources for: id={0},{1}resources={2}", id, Environment.NewLine, resources), ex);
            }

            Id = id;
            Alt = alt;
        }

        public ImageResourcesContainer Resources;

        public int? Id { get; set; }

        public string Alt { get; set; }

        public bool HasMobile
        {
            get
            {
                return Resources?.HasMobile == true;
            }
        }



        public object Clone()
        {
            return new CmsImage()
            {
                Id = Id,
                Alt = Alt,
                Resources = (ImageResourcesContainer)Resources.Clone()
            };
        }

        public class ImageResourcesContainer : ICloneable
        {
            public ImageResources Desktop { get; set; }
            public ImageResources Mobile { get; set; }
            public object Clone()
            {
                return new ImageResourcesContainer()
                {
                    Desktop = (ImageResources)Desktop.Clone(),
                    Mobile = (ImageResources)Mobile.Clone()
                };
            }

            public bool HasMobile
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(Mobile?.FileName);
                }
            }
        }

        public class ImageResources : ICloneable
        {
            public int Height { get; set; }
            public int Width { get; set; }
            public string FileName { get; set; }

            public object Clone()
            {
                return new ImageResources()
                {
                    Height = Height,
                    Width = Width,
                    FileName = FileName
                };
            }
        }

        public string GetImageUrl()
        {
            return GetImageUrl(false, false, false);
        }

        public string GetImageMobileUrl()
        {
            return GetImageUrl(true, true, false);
        }

        public string GetImageMobileUrl(bool isFullUrl)
        {
            return GetImageUrl(true, true, isFullUrl);
        }

        public string GetImageMobileUrl(bool isDefaultMobile, bool isFullUrl)
        {
            return GetImageUrl(true, isDefaultMobile, isFullUrl);
        }

        public string GetImageUrl(bool isFullUrl)
        {
            return GetImageUrl(false, false, isFullUrl);
        }

        public ImageResources GetResource(bool isMobile, bool isMobileDefault)
        {
            if (Resources != null)
            {
                if (isMobile)
                {
                    if (Resources.HasMobile || isMobileDefault)
                        return Resources.Mobile;
                }

                return Resources.Desktop;
            }

            return null;
        }


        public string GetImageUrl(bool isMobile, bool isMobileDefault, bool isFullUrl)
        {
            if (Id == 0)
                return null;

            var resource = GetResource(isMobile, isMobileDefault);
            if (resource != null)
            {
                if (isFullUrl)
                    return $"{Config.GetAppSettings("ForesterCms.ImagesSiteUrl")}uploadedimages/{resource.FileName}";

                return $"uploadedimages/{resource.FileName}";
            }

            return null;
        }
    }
}
