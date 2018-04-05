
using Microsoft.AspNetCore.Mvc.Filters;
using Hong.Common.Extendsion;

namespace Hong.Common.Html
{
    public class HtmlAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var pu = ServiceProvider.GetService<UrlUnity>();
            var html = pu?.GetCacheHtml();

            if (html == null)
            {
                return;
            }

            context.Result = new Microsoft.AspNetCore.Mvc.ContentResult
            {
                Content = html,
                StatusCode = 200,
                ContentType = "text/html"
            };
        }
    }
}
