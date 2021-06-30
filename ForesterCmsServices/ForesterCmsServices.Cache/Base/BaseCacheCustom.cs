using Common.Utils.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Cache.Base
{
    public abstract class BaseCacheCustom<TInstance> where TInstance : BaseCacheCustom<TInstance>, new()
    {
        #region Static

        private static Action _refreshAction;
        private static string entityTypeAlias = new TInstance().EntityTypeAlias;
        private static readonly object _instanceLock = new object();

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

                            CacheHelper.GetOrAddWithRemove(
                                entityTypeAlias,
                                () => instance,
                                60 * 120,
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

        #endregion

        protected virtual void SetOnRefresh(Action refreshAction)
        {
            _refreshAction = refreshAction;
        }

        private static void BaseRefreshAction(string key, object value)
        {
            CacheHelper.m_primitivesCache.RemoveByBaseKey(entityTypeAlias);

            if (_refreshAction != null)
                _refreshAction();
        }

        protected TResult GetOrAdd<TResult>(Func<TResult> method, double seconds, bool isSliding, params object[] keyParams) where TResult : class
        {
            return CacheHelper.GetOrAdd<TResult>(EntityTypeAlias, method, seconds, isSliding, keyParams);
        }

        public void Remove(params object[] keyParams)
        {
            CacheHelper.Remove(EntityTypeAlias, keyParams);
        }

        protected virtual void Init() { }

        public abstract string EntityTypeAlias { get; }
    }
}
