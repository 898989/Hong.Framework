
namespace Hong.Cache
{
    /// <summary>缓存过期方式
    /// </summary>
    public enum ExpirationMode
    {
        /// <summary>
        /// 永不过期
        /// </summary>
        None,

        /// <summary>
        /// 滑动过期
        /// </summary>
        Sliding,

        /// <summary>
        /// 绝对过期
        /// </summary>
        Absolute
    }
}
