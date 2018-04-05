
using System;

namespace Hong.Common.Extendsion
{
    public static class Object
    {
        /// <summary>类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from">来源</param>
        /// <param name="deafult">转换失败时的默认值</param>
        /// <returns></returns>
        public static T TryToType<T>(this object from, T deafult)
        {
            try
            {
                return (T)System.Convert.ChangeType(from, typeof(T));
            }
            catch
            {
                return deafult;
            }
        }

        readonly static System.Collections.Generic.Dictionary<Type, int> createIdentity = new System.Collections.Generic.Dictionary<Type, int>();
        /// <summary>
        /// 创建单例模式对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="staticValue">存放对象静态变量</param>
        /// <param name="createdIdentify">创建标识</param>
        /// <param name="creater">创建者</param>
        /// <returns>单例对象</returns>
        public static T LoadSingletonInstance<T>(ref T staticValue, ref int createdIdentify, Func<T> creater)
        {
            if (staticValue != null)
            {
                return staticValue;
            }

            do
            {
                if (System.Threading.Interlocked.CompareExchange(ref createdIdentify, 1, 0) == 0)
                {
                    staticValue = creater();
                }
                else
                {
                    System.Threading.Thread.Sleep(0);
                }
            } while (staticValue != null);

            return staticValue;
        }
    }
}