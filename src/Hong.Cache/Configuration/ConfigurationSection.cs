
namespace Hong.Cache.Configuration
{
    public class ConfigurationSection
    {
        /// <summary>配置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>是否启用缓存
        /// </summary>
        public bool Enabled { get; set; }
    }
}
