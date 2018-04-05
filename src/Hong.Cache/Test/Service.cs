using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Hong.Test.Model
{
    public class Service
    {
        public long ID { get; set; }

        public T Load<T>(string field)
        {
            return default(T);
        }
    }
}
