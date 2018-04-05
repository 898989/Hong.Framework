using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hong.MQ
{
    public interface IPublish
    {
        /// <summary>发送消息
        /// </summary>
        /// <param name="msg">消息内容</param>
        void SendMsg(string msg);
    }
}
