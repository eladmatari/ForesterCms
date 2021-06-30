using Common.Utils.Standard;
using ForesterCmsServices.Objects.Cache;
using ForesterCmsServices.Objects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Cache.Base
{
    public abstract class BaseCacheLazyList<TInstance, TEntity> where TInstance : BaseCacheLazyList<TInstance, TEntity>, new()
                                                    where TEntity : BaseCmsEntity, new()
    {
        #region Static

        //private static Action<string, object, Caching.CacheItemRemovedReason> _refreshAction;
        //private static readonly object _instanceLock = new object();
        private static readonly object _itemsLock = new object();

        private static TInstance _instance = new TInstance();
        public static TInstance Instance
        {
            get
            {
                return _instance;
            }
        }

        private static string entityTypeAlias = Instance.EntityTypeAlias;

        #endregion

        public BaseCacheLazyList()
        {
            CacheManager.OnCacheRefresh += CacheManager_OnCacheRefresh;
        }

        private void CacheManager_OnCacheRefresh(object sender, CacheRefreshEventArgs e)
        {
            if (e.EntityInfoName == EntityTypeAlias && e.EntityInfoId != null && e.ObjId != null && e.LCID != null)
            {
                string cacheKey = GetCacheKey((int)e.EntityInfoId, (int)e.ObjId, (int)e.LCID);

                CacheHelper.m_primitivesCache.Remove(cacheKey);
            }
        }

        public abstract string EntityTypeAlias { get; }

        public DateTime UpdateDate { get; protected set; }

        public List<TEntity> GetItems(int nsid, int objId, int lcid)
        {
            string cacheKey = GetCacheKey(nsid, objId, lcid);

            var items = CacheHelper.m_primitivesCache.Get(cacheKey);

            if (items == null)
            {
                lock (_itemsLock)
                {
                    if (items == null)
                    {
                        items = GetItemsFromDB(nsid, objId, lcid);

                        CacheHelper.GetOrAdd(
                            cacheKey,
                            () => items,
                            60 * 120,
                            true
                        );
                    }
                }
            }

            return (List<TEntity>)items;
        }

        private string GetCacheKey(int nsid, int objId, int lcid)
        {
            return string.Format("{0}:{1}_{2}_{3}", EntityTypeAlias, nsid, objId, lcid);
        }

        protected abstract List<TEntity> GetItemsFromDB(int nsid, int objId, int lcid);
    }
}
