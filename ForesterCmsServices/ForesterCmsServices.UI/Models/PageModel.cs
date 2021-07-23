using Common.Utils.Standard;
using ForesterCmsServices.Objects.Core;
using ForesterCmsServices.UI.Base;
using ForesterCmsServices.UI.General;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Models
{
    public class PageModel : BasePageModel
    {
        public List<string> Meta { get; internal set; }
        internal Dictionary<string, ViewPlaceHolder> PlaceHolders { get; set; }
        public List<BranchObject> BranchObjects { get; internal set; }
        public ViewPlaceHolder GetPlaceHolder(string key)
        {
            ViewPlaceHolder ph;
            if (key != null && PlaceHolders.TryGetValue(key.ToLower(), out ph))
                return ph;

            return null;
        }

        public List<ViewPlaceHolder> GetPlaceHolders()
        {
            return PlaceHolders.Values.ToList();
        }


        public List<BranchObject> PageObjects { get; internal set; }

        public List<string> _htmlToHead = new List<string>();



        public void AddToHead(string html)
        {
            _htmlToHead.Add(html);
        }

        public string GetHeadHtml()
        {
            return string.Join("\n", _htmlToHead);
        }
    }
}
