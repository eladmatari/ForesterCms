using Common.Utils.Logging;
using ForesterCmsServices.Objects.Cache;
using ForesterCmsServices.Objects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Cache.Base
{
    public abstract class BaseStaticList<TInstance, TEntity> where TInstance : BaseStaticList<TInstance, TEntity>, new()
                                                    where TEntity : BaseCmsEntity, new()
    {
        private static TInstance _instance;
        public static TInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                        {
                            var instance = new TInstance();
                            instance.LoadItems();
                            CacheManager.OnCacheRefresh -= instance.CacheManager_OnCacheRefresh;
                            CacheManager.OnCacheRefresh += instance.CacheManager_OnCacheRefresh;
                            _instance = instance;
                        }
                    }
                }

                return _instance;
            }
        }

        private static readonly object _instanceLock = new object();
        private static readonly object _itemsLock = new object();

        protected Dictionary<string, TEntity> ItemsDict { get; set; }
        public List<TEntity> Items { get; private set; }
        protected abstract string[] EntityInfosNames { get; set; }
        public DateTime UpdateDate { get; protected set; }

        public virtual TEntity GetItem(int objId, int lcid)
        {
            TEntity item;
            ItemsDict.TryGetValue(GetDictionaryKey(objId, lcid), out item);
            return item;
        }

        public void LoadItems()
        {
            lock (_itemsLock)
            {
                Items = GetItemsAllFromDB();
                SetDictionaryItems();
                UpdateDate = DateTime.Now;
            }

            Logger.Debug("cache loaded: " + typeof(TInstance).Name);
        }

        private void CacheManager_OnCacheRefresh(object sender, CacheRefreshEventArgs e)
        {
            if (e.ObjId != null && EntityInfosNames.Any(i => i.Equals(e.EntityInfoName, StringComparison.InvariantCultureIgnoreCase)))
            {
                try
                {
                    lock (_itemsLock)
                    {
                        var lcidList = new List<int>();
                        if (e.LCID != null)
                        {
                            lcidList.Add(e.LCID.Value);
                        }
                        else
                        {
                            lcidList.AddRange(Items.Select(i => i.LCID).Distinct());
                        }

                        foreach (var lcid in lcidList)
                        {
                            var firstItem = Items.FirstOrDefault();
                            int? nsId = firstItem != null ? (int?)firstItem.EntityInfoId : null;

                            var oldItems = GetRefreshedItems(e.EntityInfoName, (int)e.ObjId, lcid);

                            foreach (var oldItem in oldItems)
                            {
                                RemoveDictionaryItem(oldItem);
                            }

                            if (nsId == e.EntityInfoId)
                            {
                                var newItem = GetItemFromDB((int)e.ObjId, lcid);

                                if (newItem != null)
                                {
                                    SetDictionaryItem(newItem);
                                }
                            }
                            else
                            {
                                foreach (var oldItem in oldItems)
                                {
                                    var newItem = GetItemFromDB((int)oldItem.ObjId, (int)oldItem.LCID);
                                    if (newItem != null)
                                    {
                                        SetDictionaryItem(newItem);
                                    }
                                }
                            }
                        }

                        SetItemsFromDictionary();
                        UpdateDate = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    if (e.ReTryCounter >= 50)
                    {
                        Logger.Fatal(ex, "fatal error refreshing static list item, removing entire list");
                        _instance = null;
                    }
                    else
                    {
                        Logger.Error(ex, string.Format("error refreshing static list item, retry: {0}", e.ReTryCounter));
                        e.ReTryCounter++;

                        Task.Run(() =>
                        {
                            Task.Delay(5000).Wait();
                            CacheManager_OnCacheRefresh(sender, e);
                        });
                    }
                }
            }
        }

        private void SetItemsFromDictionary()
        {
            Items = ItemsDict.Values.ToList();
        }

        private void SetDictionaryItem(TEntity item)
        {
            ItemsDict[GetDictionaryKey(item)] = item;
        }

        private void RemoveDictionaryItem(TEntity item)
        {
            ItemsDict.Remove(GetDictionaryKey(item));
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

        protected abstract TEntity GetItemFromDB(int objId, int lcid);

        protected abstract List<TEntity> GetRefreshedItems(string entityTypeAlias, int objId, int lcid);
    }
}
