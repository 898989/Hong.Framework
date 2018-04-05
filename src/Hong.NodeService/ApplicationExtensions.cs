using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hong.NodeService
{
    public class ApplicationExtensions
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void AddNodeServices(IServiceCollection services)
        {
            services.AddNodeServices();
            /*services.AddMvc();*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, INodeServices nodeServices)
        {
            string output = "here";

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(next => async context =>
            {
                var requestPath = context.Request.Path.Value;
                output += requestPath;
                /*if (requestPath.StartsWith("/ts/") && requestPath.EndsWith(".ts")) {*/
                if (true)
                {
                    requestPath = "./ts/demo.ts";

                    var fileInfo = env.WebRootFileProvider.GetFileInfo(requestPath);
                    if (fileInfo.Exists)
                    {
                        output += "exists";
                        Console.WriteLine("Hello World");
                        output += fileInfo.PhysicalPath;
                        var transpiled = await nodeServices.InvokeAsync<string>("./node_modules/typescript/lib/tsc", fileInfo.PhysicalPath);
                        /*                        var transpiled = await nodeServices.InvokeAsync<string>("./Node/transpilation", fileInfo.PhysicalPath);*/
                        await context.Response.WriteAsync(output);
                        return;
                    }
                }

                // Not a JS file, or doesn't exist - let some other middleware handle it
                await next.Invoke(context);
            });

            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(output);
            });
        }
    }
}
