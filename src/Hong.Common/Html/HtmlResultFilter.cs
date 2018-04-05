using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using Hong.Common.Extendsion;

namespace Hong.Common.Html
{
    public class HtmlResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var urlInfo = ServiceProvider.GetService<UrlUnity>()?.UrlInfo;

            if (urlInfo == null || urlInfo.Setting.ServerCacheTime == 0)
            {
                return;
            }

            context.HttpContext.Response.Body = new HtmlResponseStream(context.HttpContext.Response.Body, urlInfo);
            context.HttpContext.Response.OnCompleted(() =>
            {
                ((HtmlResponseStream)context.HttpContext.Response.Body).Save();
                return Task.FromResult(0);
            });
        }
    }
}
