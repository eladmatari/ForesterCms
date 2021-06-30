using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.Cache
{
    public static class MemoryCacheExtensions
    {
        public static void RemoveByBaseKey(this MemoryCache memoryCache, string baseKey)
        {
            var keys = memoryCache.Where(i => i.Key.StartsWith(baseKey)).Select(i => i.Key).ToList();

            foreach (var key in keys)
            {
                memoryCache.Remove(key);
            }
        }
    }
}
