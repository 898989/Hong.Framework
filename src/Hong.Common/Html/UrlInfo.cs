using Hong.Common.Extendsion;
using System;

namespace Hong.Common.Html
{
    public class UrlInfo
    {
        /// <summary>静态文件全名
        /// </summary>
        public string HtmlFile = null;

        string _Content = null;
        /// <summary>静态文件HTML内容
        /// </summary>
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                _Content = value;
                CacheCreateTime = new TimeSpan(DateTime.Now.Ticks);
            }
        }

        /// <summary>URL唯一KEY
        /// </summary>
        public string Key = null;

        /// <summary>缓存缓存时间
        /// </summary>
        public TimeSpan CacheCreateTime = new TimeSpan(DateTime.Now.Ticks);

        bool _Overdue = false;
        /// <summary>是否过期
        /// </summary>
        public bool Overdue
        {
            get
            {
#if DEBUG
                return true;
#else

                if (_Overdue || Setting.ServerCacheTime == 0)
                {
                    return true;
                }

                _Overdue = new TimeSpan(DateTime.Now.Ticks).Subtract(CacheCreateTime).Seconds > Setting.ServerCacheTime;

                return _Overdue;
#endif
            }
            set
            {
                _Overdue = value;
            }
        }

        private UrlSettingItem _urlSetting = null;
        /// <summary>设置项
        /// </summary>
        public UrlSettingItem Setting
        {
            get
            {
                _urlSetting = _urlSetting ?? ServiceProvider.GetService<UrlSettings>().Get(Key);

                return _urlSetting;
            }
        }
    }
}
