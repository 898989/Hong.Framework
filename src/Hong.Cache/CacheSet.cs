using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hong.Cache
{
    public class CacheSet
    {
        static readonly IDictionary<Type, CacheSetAttribute> _settnigs = new Dictionary<Type, CacheSetAttribute>();

        public static CacheSetAttribute GetSet(Type type)
        {
            CacheSetAttribute set = null;

            if(_settnigs.TryGetValue(type,out set))
            {
                return set;
            }

            lock (_settnigs)
            {
                set = type.GetTypeInfo().GetCustomAttribute<CacheSetAttribute>();
                _settnigs.Add(type, set);
            }

            return set;
        }
    }
}
