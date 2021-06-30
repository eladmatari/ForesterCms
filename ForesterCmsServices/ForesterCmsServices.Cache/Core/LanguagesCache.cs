using ForesterCmsServices.Cache.Base;
using ForesterCmsServices.Objects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Cache.Core
{
    public class LanguagesCache : BaseCacheList<LanguagesCache, CmsLanguage>
    {
        private Dictionary<string, CmsLanguage> _aliasLangs = null;

        public override string EntityTypeAlias { get { return "language"; } }

        protected override List<CmsLanguage> GetItemsAllFromDB()
        {
            throw new NotImplementedException();
        }

        protected override void OnLoaded()
        {
            _aliasLangs = Items.GroupBy(i => (i.Alias ?? "").Trim().ToLower()).ToDictionary(i => i.Key, i => i.First());
        }

        public CmsLanguage GetItem(string alias)
        {
            CmsLanguage lang;
            _aliasLangs.TryGetValue(alias ?? "", out lang);
            return lang;
        }
    }
}
