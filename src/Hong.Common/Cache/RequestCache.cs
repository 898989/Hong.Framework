using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hong.Common.Cache
{
    /// <summary>请求域缓存
    /// </summary>
    public class RequestCache : Cache<string>
    {
        IDictionary<object, object> _container = null;

        public RequestCache(IHttpContextAccessor httpContextAccessor)
        {
            _container = httpContextAccessor.HttpContext.Items;
        }

        public override T Get<T>(string key)
        {
            object obj;

            if (_container.TryGetValue(key, out obj))
            {
                return (T)obj;
            }

            return default(T);
        }

        public override void Remove(string key)
        {
            if (_container.ContainsKey(key))
                _container.Remove(key);
        }

        public override void Set(string key, object value)
        {
            if (_container.ContainsKey(key))
            {
                _container[key] = value;
            }
            else
            {
                _container.Add(key, value);
            }
        }
    }
}
