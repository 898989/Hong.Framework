using System.IO;
using System.Linq;
using System.Diagnostics;
using Hong.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace WEB_NETCORE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //bool isService = true;      //windows下注册成功系统服务

            //if (Debugger.IsAttached || args.Contains("--console"))
            //{
            //    isService = false;
            //}

            var pathToContentRoot = Directory.GetCurrentDirectory();
            //if (isService)
            //{
            //    var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            //    pathToContentRoot = Path.GetDirectoryName(pathToExe);
            //}

            var host = new Microsoft.AspNetCore.Hosting.WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(pathToContentRoot)
                .UseIISIntegration()
                //.UseUrls("http://192.168.1.103:5000")
                .UseStartup<Startup>()
                //.UseApplicationInsights()
                .Build();

            //if (isService)
            //{
            //    host.RunAsCustomService();
            //}
            //else
            //{
            host.Run();
            //}
        }
    }
}
