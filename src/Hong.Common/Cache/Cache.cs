using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hong.Common.Cache
{
    public abstract class Cache<key>
    {
        public abstract T Get<T>(key key);

        public abstract void Set(key key, object value);

        public abstract void Remove(key key);
    }
}
