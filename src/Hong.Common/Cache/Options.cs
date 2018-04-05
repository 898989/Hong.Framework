using System;
using System.Collections.Generic;

namespace Hong.Common.Cache
{
    /// <summary>操作项缓存
    /// </summary>
    public class Options : Cache<int>
    {
        Dictionary<int, object> _options = new Dictionary<int, object>();

        /// <summary>获取操作项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">操作项编号</param>
        /// <returns></returns>
        public override T Get<T>(int key)
        {
            object obj = null;

            if (_options.TryGetValue(key, out obj))
            {
                return (T)obj;
            }

            return default(T);
        }

        /// <summary>不支持此操作
        /// </summary>
        /// <param name="key"></param>
        public override void Remove(int key)
        {
            throw new NotImplementedException();
        }

        /// <summary>添加操作项
        /// </summary>
        /// <param name="key">操作项编号</param>
        /// <param name="value">值</param>
        public override void Set(int key, object value)
        {
            if (_options.ContainsKey(key))
            {
                _options[key] = value;
                return;
            }

            lock (_options)
            {
                if (_options.ContainsKey(key))
                {
                    _options[key] = value;
                    return;
                }

                _options.Add(key, value);
            }
        }
    }
}
