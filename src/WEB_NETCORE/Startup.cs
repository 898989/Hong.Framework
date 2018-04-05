using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hong.Service.Repository;
using Hong.Mvc;
using Hong.WebSocket;
using WEB_NETCORE.Samples;
using Microsoft.AspNetCore.Mvc;

namespace WEB_NETCORE
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ////只允许HTTPS请求
            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());
            //});

            /*
            //注册单例模式，整个应用程序周期内ITodoRepository接口的示例都是TodoRepository1的一个单例实例
            services.AddSingleton<ITodoRepository, TodoRepository1>();
            services.AddSingleton(typeof(ITodoRepository), typeof(TodoRepository1));  // 等价形式

            //注册特定实例模型，整个应用程序周期内ITodoRepository接口的示例都是固定初始化好的一个单例实例

            services.AddInstance<ITodoRepository>(new TodoRepository2());
            services.AddInstance(typeof(ITodoRepository), new TodoRepository2());  // 等价形式

            //注册作用域型的类型，在特定作用域内ITodoRepository的示例是TodoRepository3
            services.AddScoped<ITodoRepository, TodoRepository3>();
            services.AddScoped(typeof(ITodoRepository), typeof(TodoRepository3));// 等价形式

            //获取该ITodoRepository实例时，每次都要实例化一次TodoRepository4类
            services.AddTransient<ITodoRepository, TodoRepository4>();
            services.AddTransient(typeof(ITodoRepository), typeof(TodoRepository));// 等价形式

            //如果要注入的类没有接口，那你可以直接注入自身类型，比如：
            services.AddTransient<LoggingHelper>();
            */

            services.AddHong();
            services.AddLogging();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddWebSocket(typeof(WebSocketMessageHandle));

            // Add framework services.
            services.AddMvc();
            services.AddSingleton(typeof(OrderDetailRepository));
            services.AddSingleton(typeof(OrderRepository));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseHong();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseHongWebSocket();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}",
                    defaults:"/index.html");
            });
        }
    }
}
