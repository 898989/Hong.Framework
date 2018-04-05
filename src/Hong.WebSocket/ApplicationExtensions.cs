using Hong.Common.Extendsion;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Hong.WebSocket
{
    public static class ApplicationExtensions
    {
        /// <summary>添加支持WebSocket
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="messageHandle">消息接收处理类，继承接口<see cref="IMessageHandle"/></param>
        public static void AddWebSocket(this IServiceCollection services,Type iMessageHandle)
        {
            services.AddSingleton(typeof(IMessageHandle), iMessageHandle);
            services.AddSingleton(typeof(WebSocketSessionManger));
            services.AddScoped(typeof(WebSocketHandle));
        }

        public static void UseHongWebSocket(this IApplicationBuilder app)
        {
            app.UseWebSockets();

            app.Use((context, next) =>
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    return next();
                }

                return Common.Extendsion.ServiceProvider.GetRequestServices<WebSocketHandle>().Process(context);
            });

            //app.Use(next => async context =>
            //{
            //    if (!context.WebSockets.IsWebSocketRequest)
            //    {
            //        await next.Invoke(context);
            //        return;
            //    }

            //    await ServiceProvider.GetRequestServices<WebSocketHandle>().Process(context);
            //});
        }
    }
}
