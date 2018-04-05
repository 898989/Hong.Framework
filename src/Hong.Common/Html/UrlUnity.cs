using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using Hong.Common.Extendsion;
using Microsoft.Extensions.Logging;

namespace Hong.Common.Html
{
    public class UrlUnity
    {
        readonly static UrlCache HtmlCache = new UrlCache();
        readonly static ILogger Log = ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger("Html");

        private HttpContext _context = null;
        private UrlInfo _htmlInfo = null;

        public UrlUnity(IHttpContextAccessor httpContextAccessor)
        {
            _context = httpContextAccessor.HttpContext;
        }

        /// <summary>请求URL信息
        /// </summary>
        public UrlInfo UrlInfo
        {
            get
            {
                if (string.IsNullOrEmpty(UrlKey))
                {
                    return null;
                }

                _htmlInfo = _htmlInfo ?? HtmlCache.Get(UrlKey);

                if (_htmlInfo == null)
                {
                    _htmlInfo = ServiceProvider.GetService<UrlInfo>();
                    _htmlInfo.Key = this.UrlKey;

                    if (_htmlInfo.Setting.ServerCacheTime > 0)
                        HtmlCache.Set(UrlKey, _htmlInfo);
                }

                return _htmlInfo;
            }
        }

        private string _urlKey = null;
        /// <summary>请求URL KEY
        /// </summary>
        public string UrlKey
        {
            get
            {
                if (_urlKey == null)
                {
                    _urlKey = _context.Request.Path;

                    if (_urlKey == "/")
                    {
                        _urlKey = "/index.html";
                    }
                    else if (_urlKey.EndsWith(".html"))
                    {
                        _urlKey = _urlKey.TrimEnd('/') + ".html";
                    }
                    else
                    {
                        _urlKey = string.Empty;
                    }
                }

                return _urlKey;
            }
        }

        /// <summary>静态化完整路径
        /// </summary>
        public string HtmlFile
        {
            get
            {
                var urlInfo = UrlInfo;

                if (urlInfo == null)
                {
                    return string.Empty;
                }

                urlInfo.HtmlFile = urlInfo.HtmlFile ?? Directory.GetCurrentDirectory() + UrlKey;

                return urlInfo.HtmlFile;
            }
        }

        /// <summary>获取缓存HTML
        /// </summary>
        /// <returns></returns>
        public string GetCacheHtml()
        {
            var urlInfo = UrlInfo;

            if (urlInfo == null || urlInfo.Overdue)
            {
                return null;
            }

            if (urlInfo.Content != null)
            {
                return urlInfo.Content;
            }

            var fInfo = new FileInfo(HtmlFile);
            if (fInfo.Exists)
            {
                try
                {
                    urlInfo.Content = File.ReadAllText(fInfo.FullName);
                }
                catch (Exception ex)
                {
                    Log?.LogError("读取缓存文件失败", ex);
                }
            }

            return urlInfo.Content;
        }
    }
}
