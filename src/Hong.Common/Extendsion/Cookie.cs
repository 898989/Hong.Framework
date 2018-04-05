using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Hong.Common.Extendsion.Guard;

namespace Hong.Common.Extendsion
{
    public class Cookie
    {
        /// <summary>设置Cookie
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="domain"></param>
        /// <param name="expire"></param>
        public static void Set(string key, string value, string domain = null, int expire = 0)
        {
            NotNullOrEmpty(key, nameof(key));

            var reponse = ServiceProvider.CurrentHttpContext.Response;
            var option = new CookieOptions();

            if (!string.IsNullOrEmpty(domain))
            {
                option.Domain = domain;
            }

            if (expire > 0)
            {
                option.Expires = DateTime.Now.AddMinutes(expire);
            }

            reponse.Cookies.Append(key, value, option);
        }

        /// <summary>删除Cookie
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            NotNullOrEmpty(key, nameof(key));

            var reponse = ServiceProvider.CurrentHttpContext.Response;
            reponse.Cookies.Delete(key);
        }

        /// <summary>获取Cookie
        /// 对于全局cookie建议使用其方案,比如:app.UseCookieAuthentication
        /// 或自定义验证过滤器和输出过滤器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            NotNullOrEmpty(key, nameof(key));

            var request = ServiceProvider.CurrentHttpContext.Request;

            var value = request.Cookies[key];
            if (value == null)
            {
                return string.Empty;
            }

            return value;
        }
    }
}
