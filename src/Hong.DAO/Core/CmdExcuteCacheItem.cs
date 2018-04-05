using System.Collections.Concurrent;
using System.Data.Common;

namespace Hong.DAO.Core
{
    /// <summary>
    /// SQL执行命令缓存
    /// </summary>
    public class CmdExcuteCacheItem
    {
        ConcurrentStack<DbCommand> _commands = new ConcurrentStack<DbCommand>();

        public DbCommand Pop(SessionConnection conn)
        {
            DbCommand cmd = null;

            if (_commands.TryPop(out cmd))
            {
                cmd.Connection = conn.CurrentDbConnection;
                return cmd;
            }

            return conn.CurrentDbConnection.CreateCommand();
        }

        public void Push(DbCommand command)
        {
            command.Connection = null;

            _commands.Push(command);
        }
    }
}
