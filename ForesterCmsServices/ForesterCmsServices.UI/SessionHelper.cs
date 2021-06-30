using Common.Utils;
using Common.Utils.Standard;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI
{
    public static class SessionHelper
    {
        public static void SaveSessionItems()
        {
            string baseItemKey = GetItemsSessionKey();

            var itemKeys = HttpContextHelper.Current.Items.Keys
                .Where(i => (i as string)?.StartsWith(baseItemKey) == true)
                .Select(i => (string)i)
                .ToList();

            foreach (var itemKey in itemKeys)
            {
                string key = itemKey.Substring(baseItemKey.Length);
                var obj = HttpContextHelper.Current.Items[itemKey];
                if (obj != null)
                    HttpContextHelper.Current.Session.SetData(key, obj);
                else
                    HttpContextHelper.Current.Session.Remove(key);
            }
        }

        public static string GetItemsSessionKey(string key = null)
        {
            return $"Cms.Session.{key}";
        }

        public static T GetData<T>(this ISession session, string key)
        {
            if (HttpContextHelper.Current != null)
            {
                string itemsKey = GetItemsSessionKey(key);
                object obj;
                if (HttpContextHelper.Current.Items.TryGetValue(itemsKey, out obj))
                    return (T)obj;

                var dataJson = session.GetString(key);
                var data = JsonHelper.TryDeserializeNoCamel<T>(dataJson);
                HttpContextHelper.Current.Items[itemsKey] = data;

                return data;
            }
            else
            {
                var dataJson = session.GetString(key);
                return JsonHelper.TryDeserializeNoCamel<T>(dataJson);
            }
        }

        public static void SetData<T>(this ISession session, string key, T data)
        {
            var dataJson = JsonHelper.SerializeNoCamel(data);
            session.SetString(key, dataJson);

            if (HttpContextHelper.Current != null)
            {
                string itemsKey = GetItemsSessionKey(key);
                HttpContextHelper.Current.Items[itemsKey] = data;
            }
        }
    }
}
