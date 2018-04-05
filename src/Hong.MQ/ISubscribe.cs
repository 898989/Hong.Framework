using System;

namespace Hong.MQ
{
    public interface ISubscribe
    {
        /// <summary>订阅开始
        /// </summary>
        /// <param name="reject"></param>
        /// <param name="callBack"></param>
        void Start(bool reject, Action<string, string, string> callBack);

        /// <summary>暂停
        /// </summary>
        void Stop();

        /// <summary>关闭
        /// </summary>
        void Close();
    }
}
