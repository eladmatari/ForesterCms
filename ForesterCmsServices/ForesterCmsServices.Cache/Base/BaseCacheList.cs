using Common.Utils.Logging;
using Common.Utils.Standard;
using ForesterCmsServices.Objects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Cache.Base
{
    public abstract class BaseCacheList<TInstance, TEntity> where TInstance : BaseCacheList<TInstance, TEntity>, new()
                                                    where TEntity : BaseCmsEntity, new()
    {
        #region Static

        private static string entityTypeAlias = new TInstance().EntityTypeAlias;
        private static Action _refreshAction;
        private static readonly object _instanceLock = new object();
        private static readonly object _itemsLock = new object();
        public static int? CacheSeconds { get; protected set; }

        public static TInstance Instance
        {
            get
            {
                var data = CacheHelper.m_primitivesCache.Get(entityTypeAlias);

                if (data == null)
                {
                    lock (_instanceLock)
                    {
                        data = CacheHelper.m_primitivesCache.Get(entityTypeAlias);

                        if (data == null)
                        {
                            var instance = new TInstance();
                            instance.Init();
                            instance.LoadItems();
                            CacheHelper.GetOrAddWithRemove(
                                entityTypeAlias,
                                () => instance,
                                CacheSeconds != null ? CacheSeconds.Value : (60 * 120),
                                true,
                                _refreshAction
                            );

                            return instance;
                        }
                    }
                }

                return (TInstance)data;
            }
        }

        public static void Refresh()
        {
            CacheHelper.m_primitivesCache.Remove(entityTypeAlias);
            Logger.Debug("cache refreshed: " + entityTypeAlias);
        }

        public virtual void SetOnRefresh(Action refreshAction)
        {
            _refreshAction = refreshAction;
        }

        #endregion

        protected Dictionary<string, TEntity> ItemsDict { get; set; }
        public List<TEntity> Items { get; private set; }
        public abstract string EntityTypeAlias { get; }
        public DateTime UpdateDate { get; protected set; }

        public TEntity GetItem(int objId, int lcid = 0)
        {
            TEntity item;
            ItemsDict.TryGetValue(GetDictionaryKey(objId, lcid), out item);
            return item;
        }

        protected virtual void Init() { }

        protected void LoadItems()
        {
            lock (_itemsLock)
            {
                Items = GetItemsAllFromDB();
                SetDictionaryItems();
                OnLoaded();
                UpdateDate = DateTime.Now;
            }
        }

        protected virtual void OnLoaded() { }

        private void SetDictionaryItem(TEntity item)
        {
            ItemsDict[GetDictionaryKey(item)] = item;
        }

        private void SetDictionaryItems()
        {
            ItemsDict = new Dictionary<string, TEntity>();
            foreach (var item in Items)
            {
                SetDictionaryItem(item);
            }
        }

        private string GetDictionaryKey(TEntity item)
        {
            return GetDictionaryKey(item.ObjId, item.LCID);
        }

        private string GetDictionaryKey(int objId, int lcid)
        {
            return objId + "_" + lcid;
        }

        protected abstract List<TEntity> GetItemsAllFromDB();
    }
}
