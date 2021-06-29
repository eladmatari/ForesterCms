using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Utils.Standard
{
    public static class LockObjFactory
    {
        private static readonly object _getLockObj = new object();
        private static readonly Dictionary<string, object> _lockObjsDict = new Dictionary<string, object>();

        public static object Get(string key)
        {
            object obj;
            if (!_lockObjsDict.TryGetValue(key, out obj))
            {
                lock (_getLockObj)
                {
                    if (!_lockObjsDict.TryGetValue(key, out obj))
                    {
                        obj = _lockObjsDict[key] = new object();
                    }
                }
            }

            return obj;
        }
    }
}
