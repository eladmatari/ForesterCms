using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Utils.Standard
{
    public static class JsonHelper
    {
        private static readonly JsonSerializerSettings _defaultSerializeSettings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _defaultSerializeSettings);
        }

        public static string SerializeNoCamel(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T TryDeserialize<T>(string json, T defaultResult = default(T))
        {
            if (json != null)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(json, _defaultSerializeSettings);
                }
                catch (Exception ex)
                {
                    Config.Logger?.Error(ex, $"error deserializing {typeof(T).FullName}, json: {json}");
                }
            }

            return defaultResult;
        }

        public static T TryDeserializeNoCamel<T>(string json, T defaultResult = default(T))
        {
            if (json != null)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (Exception ex)
                {
                    Config.Logger?.Error(ex, $"error deserializing {typeof(T).FullName}, json: {json}");
                }
            }

            return defaultResult;
        }

        public static string GetJsonProperty(string s)
        {
            if (s == null)
                return s;

            var sArr = s.Replace('_', '-').Replace(' ', '-').Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < sArr.Length; i++)
            {
                if (i > 0 && sArr[i].Length > 1)
                    sArr[i] = sArr[i].Substring(0, 1).ToUpper() + sArr[i].Substring(1);
            }

            return string.Join("", sArr);
        }
    }
}
