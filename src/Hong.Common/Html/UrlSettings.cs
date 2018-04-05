using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hong.Common.Html
{
    public class UrlSettings
    {
        Dictionary<string, UrlSettingItem> _settings = new Dictionary<string, UrlSettingItem>();
        UrlSettingItem _null = new UrlSettingItem();

        public UrlSettings()
        {
            LoadSettings();
        }

        void LoadSettings()
        {
            Set("null", _null);
        }


        public UrlSettingItem Get(string url)
        {
            string key = SettingKey(url);

            UrlSettingItem item = null;

            if (_settings.TryGetValue(key, out item))
            {
                return item;
            }

            return _null;
        }

        static Regex _reg_settingKey = new Regex("/(.*?)/");
        private string SettingKey(string url)
        {
            Match m = _reg_settingKey.Match(url);
            return m.Groups.Count > 1 ? m.Groups[1].Value : "null";
        }

        void Set(string settingKey, UrlSettingItem setting)
        {
            if (_settings.ContainsKey(settingKey))
            {
                return;
            }

            _settings.Add(settingKey, setting);
        }
    }
}
