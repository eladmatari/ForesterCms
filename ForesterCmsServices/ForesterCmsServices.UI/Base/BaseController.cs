using Common.Utils.Logging;
using ForesterCmsServices.Logic;
using ForesterCmsServices.Objects.Core;
using ForesterCmsServices.UI.General;
using ForesterCmsServices.UI.Models;
using ForesterCmsServices.UI.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Base
{
    public class AdditionalPageControlsEventArgs : EventArgs
    {
        public List<CmsComponent> Components;
        public AdditionalPageControlsEventArgs()
        {

        }
    }

    public class BaseController<T> : Controller where T : PageModel, new()
    {
        public RouterData RouterData { get; private set; }

        public T Model { get; set; }
        protected PageActionAttribute PageActionAttr { get; private set; }

        protected string _canonicalUrl;

        public delegate void AditionalPageControlsEventHandlerHandler(object sender, AdditionalPageControlsEventArgs e);
        public event AditionalPageControlsEventHandlerHandler AdditionalPageControlsLoad;
        protected static List<MetaTemplate> MetaTemplates { get; set; }

        static BaseController()
        {
            MetaTemplates = new List<MetaTemplate>();
            MetaTemplates.Add(new MetaTemplate() { Id = 1, Type = MetaTemplate.TemplateType.Title, Template = @"<title>{value}</title>" });
            MetaTemplates.Add(new MetaTemplate() { Id = 2, Type = MetaTemplate.TemplateType.Keywords, Template = @"<meta name=""keywords"" content=""{value}"">" });
            MetaTemplates.Add(new MetaTemplate() { Id = 3, Type = MetaTemplate.TemplateType.Description, Template = @"<meta name=""description"" content=""{value}"">" });
            MetaTemplates.Add(new MetaTemplate() { Id = 4, Type = MetaTemplate.TemplateType.Robots, Template = @"<meta name=""robots"" content=""{value}"">" });
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this.PageActionAttr = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo
                .GetCustomAttributes(typeof(PageActionAttribute), false)
                .Select(i => (PageActionAttribute)i)
                .FirstOrDefault();

            if (PageActionAttr == null)
                throw new Exception($"action is missing attribute {nameof(PageActionAttribute)}");

            if (context.HttpContext.Request.Query["listph"] == "1")
            {
                context.Result = Content(string.Join(",", PageActionAttr.PlaceHolders));
                return;
            }

            RouterData = Router.Data;

            if (RouterData == null)
            {
                Logger.Info($"RouterData is empty {Request.AbsoluteUrl()}");
                return;
            }

            Router.Data.PageModel = Model = new T();
            if (PageActionAttr.LoadPageObjects)
                Model.PageObjects = CmsServicesManager.Core.GetObjects(RouterData.BranchId, RouterData.LCID);

            var cmsComponents = CmsServicesManager.Core.GetPageControlsByBoneId(RouterData.BranchId, RouterData.LCID);
            AdditionalPageControlsLoad?.Invoke(this, new AdditionalPageControlsEventArgs() { Components = cmsComponents });

            Model.PlaceHolders = PageActionAttr.PlaceHolders.GroupBy(i => i.ToLower()).ToDictionary(i => i.Key, i => new ViewPlaceHolder()
            {
                Alias = i.Key,
                Components = new List<CmsComponent>()
            });

            foreach (var cmsComponent in cmsComponents.OrderBy(i => i.Sort))
            {
                ViewPlaceHolder placeHolder;
                if (cmsComponent.PlaceHolder == null || !Model.PlaceHolders.TryGetValue(cmsComponent.PlaceHolder?.ToLower(), out placeHolder))
                    continue;

                if (cmsComponent.Display != 0 && (cmsComponent.Display == 2) != RouterData.IsMobile)
                    continue;

                placeHolder.Components.Add(cmsComponent);
            }

            OnLoad(context);
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (Model != null && PageActionAttr?.LoadMeta == true)
                Model.Meta = GetMetaTags();

            base.OnActionExecuted(context);
        }

        protected virtual List<string> GetMetaTags()
        {
            var templates = MetaTemplates;
            var metaValues = RouterData.BranchProperties.Meta;
            var tags = new List<string>();

            foreach (var template in templates)
            {
                string val = metaValues.GetValue(template.Type);
                val = ProcessMetaValue(template.Type, val);
                if (!string.IsNullOrWhiteSpace(val))
                    tags.Add(template.Template.Replace("{value}", val));
            }

            tags.Add($"<link rel=\"canonical\" href=\"{GetCanonicalUrl()}\" />");

            return tags;
        }

        protected virtual string ProcessMetaValue(MetaTemplate.TemplateType type, string val)
        {
            return val;
        }

        protected virtual string GetCanonicalUrl()
        {
            if (_canonicalUrl != null)
                return _canonicalUrl;

            return Request.AbsoluteUrlNoQueryString();
        }

        protected virtual void OnLoad(ActionExecutingContext context) { }
    }

    public class BaseController : BaseController<PageModel>
    {

    }
}
