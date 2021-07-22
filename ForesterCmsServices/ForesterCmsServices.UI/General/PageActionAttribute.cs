using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.General
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PageActionAttribute : Attribute
    {
        public PageActionAttribute(string placeholdersCsv, bool loadPageObjects = false, bool loadMeta = true)
        {
            PlaceHoldersCsv = placeholdersCsv;
            LoadPageObjects = loadPageObjects;
            LoadMeta = loadMeta;
        }

        private string _placeHoldersCsv;
        public string PlaceHoldersCsv
        {
            get
            {
                return _placeHoldersCsv;
            }
            set
            {
                _placeHoldersCsv = value ?? "";
                PlaceHolders = _placeHoldersCsv.Split(',').Select(i => i.Trim()).ToArray();
            }
        }
        public string[] PlaceHolders { get; private set; }
        public bool LoadPageObjects { get; set; }
        public bool LoadMeta { get; set; }
    }
}
