using System.Collections.Generic;
using static Hong.Common.Extendsion.Guard;

namespace Hong.Cache.Core
{
    /// <summary>请求域缓存
    /// </summary>
    public class SessionCache
    {
        private Dictionary<string, object> _caches = new Dictionary<string, object>();

        public T Get<T>(string key)
        {
            NotNullOrEmpty(key, nameof(key));

            if (_caches.TryGetValue(key, out object obj))
            {
                return (T)obj;
            }

            return default(T);
        }

        public T Get<T>(string key, string region)
        {
            if (string.IsNullOrEmpty(region))
            {
                return Get<T>(key);
            }

            return Get<T>(region + "_" + key);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                _caches.Remove(key);
            }

            _caches.Remove(key);
        }

        public void Remove(string key, string region)
        {
            if (string.IsNullOrEmpty(region))
            {
                Remove(key);
            }

            Remove(region + "_" + key);
        }

        public void Set(string key, object value)
        {
            if (_caches.ContainsKey(key))
            {
                if (value is object)
                {
                    return;
                }

                _caches[key] = value;

                return;
            }

            _caches.Add(key, value);
        }

        public void Set(string key, string region, object value)
        {
            Set(region + "_" + key, value);
        }
    }
}
