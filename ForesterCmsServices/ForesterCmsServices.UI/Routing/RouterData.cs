using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForesterCmsServices.Objects.Core;
using ForesterCmsServices.UI.Models;

namespace ForesterCmsServices.UI.Routing
{
    public class RouterData
    {
        private CmsBranch _branch;
        public CmsBranch Branch
        {
            get
            {
                return _branch;
            }
            internal set
            {
                _branch = value;
                BranchId = _branch?.ObjId ?? 0;
            }
        }

        public CmsBranchProperties BranchProperties { get; internal set; }

        public int BranchId { get; internal set; }

        private CmsLanguage _language;
        public CmsLanguage Language
        {
            get
            {
                return _language;
            }
            internal set
            {
                _language = value;
                LCID = _language?.ObjId ?? 0;
            }
        }

        public int LCID { get; private set; }

        public int ObjId { get; internal set; }

        //public string ObjAlias { get; internal set; }

        private CmsEntityInfo _entityInfo;
        public CmsEntityInfo EntityInfo
        {
            get
            {
                return _entityInfo;
            }
            internal set
            {
                _entityInfo = value;
                EntityInfoId = _entityInfo?.ObjId ?? 0;
            }
        }

        public int EntityInfoId { get; internal set; }

        private DisplayPage _displayPage;
        public DisplayPage DisplayPage
        {
            get
            {
                return _displayPage;
            }
            internal set
            {
                _displayPage = value;
                if (_displayPage != null)
                {
                    RouteParams = GetRouteParams(_displayPage.View);
                }
                else
                {
                    RouteParams = null;
                }
            }
        }

        public static RouteParamsObject GetRouteParams(string view)
        {
            var routeParams = new RouteParamsObject();
            List<string> fileNameList = null;

            fileNameList = view.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            routeParams.Controller = string.Join("", fileNameList.Take(fileNameList.Count - 1));
            routeParams.Action = fileNameList.Last();

            if (!string.IsNullOrWhiteSpace(routeParams.Action) && string.IsNullOrWhiteSpace(routeParams.Controller))
                routeParams.Controller = "Home";

            return routeParams;
        }

        public RouteParamsObject RouteParams { get; private set; }

        public NameValueCollection VirtualRouteParams { get; internal set; }

        public class RouteParamsObject
        {
            public string Controller { get; set; }

            public string Action { get; set; }
            public bool IsOldSitePage { get; internal set; }
        }

        public PageModel PageModel { get; internal set; }
        public bool IsMobile { get; internal set; }
        public string Rel { get; internal set; }
        public ICmsEntity PageEntity { get; set; }
    }
}
