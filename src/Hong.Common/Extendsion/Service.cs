using Hong.Common.Cache;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Hong.Common.Extendsion
{
    public static class ServiceProvider
    {
        /// <summary>全局容器
        /// </summary>
        public static IServiceProvider PublicServiceProvider;

        /// <summary>获取当前请求内服务对象
        /// </summary>
        public static T GetRequestServices<T>()
        {
            var httpContext = CurrentHttpContext;
            if (httpContext == null)
            {
                throw new NotSupportedException("线程暂未支持");
            }

            var provider = httpContext.RequestServices as IServiceProvider;
            var obj = provider?.GetService(typeof(T));

            if (obj == null)
            {
                throw new NotSupportedException("如果类'" + typeof(T).FullName + "'需要依赖注入,请先在容器中注册");
            }

            return (T)obj;
        }

        /// <summary>请求内缓存
        /// </summary>
        public static RequestCache RequestCache => GetRequestServices<RequestCache>();

        /// <summary>当前请求上下文
        /// </summary>
        public static HttpContext CurrentHttpContext
        {
            get
            {
                var ihttpContextAccessor = PublicServiceProvider?.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                return ihttpContextAccessor?.HttpContext;
            }
        }

        /// <summary>获取全局公共服务对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            object obj = PublicServiceProvider.GetService(typeof(T));

            if (obj == null)
            {
                throw new NotSupportedException("如果类'" + typeof(T).FullName + "'需要依赖注入,请先在容器中注册");
            }

            return (T)obj;
        }
    }
}
