using System;

namespace Hong.DAO.Core
{
    public class VersionException : Exception
    {
        public override string Message
        {
            get
            {
                return "版本过低更新失败";
            }
        }
    }
}
