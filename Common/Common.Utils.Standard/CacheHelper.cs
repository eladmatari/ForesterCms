using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace Common.Utils.Standard
{
    public static class CacheHelper
    {
        public static readonly MemoryCache m_primitivesCache = MemoryCache.Default;

        public static void RemoveBaseKey(string baseKey)
        {
            var keysToRemove = m_primitivesCache.Where(i => i.Key.StartsWith(baseKey)).Select(i => i.Key).ToList();

            foreach (var keyToRemove in keysToRemove)
            {
                m_primitivesCache.Remove(keyToRemove);
            }
        }

        public static void Remove(string baseKey, params object[] keyParams)
        {
            string key = CreateCacheKey(baseKey, keyParams);

            m_primitivesCache.Remove(key);
        }

        public static string CreateCacheKey(string baseKey, params object[] keyParams)
        {
            if (keyParams.Length == 0)
                return baseKey;

            return baseKey + ":" + string.Join("_", keyParams);
        }

        public static TResult Get<TResult>(string baseKey, params object[] keyParams) where TResult : class
        {
            string key = CreateCacheKey(baseKey, keyParams);

            var result = m_primitivesCache[key] as TResult;

            return result;
        }

        public static TResult GetOrAdd<TResult>(string baseKey, Func<TResult> method, double seconds, bool isSliding, params object[] keyParams) where TResult : class
        {
            return GetOrAddWithRemove(baseKey, method, seconds, isSliding, null, keyParams);
        }

        public static TResult GetOrAddWithRemove<TResult>(string baseKey, Func<TResult> method, double seconds, bool isSliding, Action onRefresh, params object[] keyParams) where TResult : class
        {
            string key = CreateCacheKey(baseKey, keyParams);

            var result = m_primitivesCache[key] as TResult;

            if (result == null)
            {
                lock (LockObjFactory.Get($"CacheHelper:{key}"))
                {
                    result = m_primitivesCache[key] as TResult;
                    if (result == null)
                    {
                        result = method();

                        if (result != null)
                        {
                            if (isSliding)
                            {
                                m_primitivesCache.Add(
                                    key,
                                    result,
                                    new CacheItemPolicy()
                                    {
                                        SlidingExpiration = TimeSpan.FromSeconds(seconds),
                                        Priority = CacheItemPriority.Default,
                                        RemovedCallback = (args) => { onRefresh?.Invoke(); }
                                    }
                                );
                            }
                            else
                            {
                                m_primitivesCache.Add(
                                    key,
                                    result,
                                    new CacheItemPolicy()
                                    {
                                        AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(seconds),
                                        Priority = CacheItemPriority.Default,
                                        RemovedCallback = (args) => { onRefresh?.Invoke(); }
                                    }
                                );
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
