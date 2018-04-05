using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hong.DAO.Core
{
    public class DbException : Exception
    {
        string msg;

        public DbException(string msg)
        {
            this.msg = msg;
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
