using System;

namespace Hong.DAO.Core
{
    public class InvalidException : Exception
    {
        private string msg;

        public InvalidException(string name)
        {
            msg = "无效的属性或字段 [" + name + "]";
        }

        public override string Message
        {
            get
            {
                return msg;
            }
        }
    }
}
