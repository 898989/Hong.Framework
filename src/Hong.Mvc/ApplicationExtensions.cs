using Hong.Cache.Core;
using Hong.Common.Cache;
using Hong.Common.Extendsion;
using Hong.Common.Html;
using Hong.DAO.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Hong.Mvc
{
    public static class Hong
    {
        static IApplicationBuilder _App;

        /// <summary>全局应用
        /// </summary>
        public static IApplicationBuilder App
        {
            get { return _App; }
            set
            {
                Common.Extendsion.ServiceProvider.PublicServiceProvider = value.ApplicationServices;
                _App = value;
            }
        }

        public static void AddHong(this IServiceCollection services)
        {
            services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
            {
                options.Filters.Add(typeof(HtmlAuthorizationFilter), 0);
                options.Filters.Add(typeof(HtmlResultFilter), 1);
            });

            services.AddSingleton(typeof(UrlSettings));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped(typeof(UrlUnity));
            services.AddScoped(typeof(RequestCache));

            services.AddTransient(typeof(UrlInfo));

            services.AddScoped(typeof(SessionConnection));
            services.AddScoped(typeof(SessionCache));
        }

        public static void UseHong(this IApplicationBuilder app)
        {
            App = app;
            //app.Use(next => async context =>
            //{
            //    if (!context.WebSockets.IsWebSocketRequest)
            //    {
            //        await next.Invoke(context);
            //    }

            //    await ServiceProvider.GetRequestServices<WebSocketHandle>().Process(context);
            //});
        }

        //private static async Task Invoke(RequestDelegate next, HttpContext content)
        //{
        //    await next.Invoke(content);
        //}

        /// <summary>应用程序操作项
        /// </summary>
        public static Options Option { get; set; } = new Options();
    }
}
