using System.Data.Common;
using System.Collections.Concurrent;

namespace Hong.DAO.Core
{
    public class CmdModelRepository
    {
        ConcurrentStack<DbCommand> queryCommands = new ConcurrentStack<DbCommand>();
        ConcurrentStack<DbCommand> insertCommands = new ConcurrentStack<DbCommand>();
        ConcurrentStack<DbCommand> deleteCommands = new ConcurrentStack<DbCommand>();
        ConcurrentStack<DbCommand> updateCoommands = new ConcurrentStack<DbCommand>();

        public DbCommand Pop(SQLAction action, SessionConnection conn)
        {
            DbCommand cmd = null;

            if (GetCommands(action).TryPop(out cmd))
            {
                cmd.Connection = conn.CurrentDbConnection;
                return cmd;
            }

            return conn.CurrentDbConnection.CreateCommand();
        }

        public void Push(SQLAction action, DbCommand command)
        {
            command.Connection = null;

            GetCommands(action).Push(command);
        }

        private ConcurrentStack<DbCommand> GetCommands(SQLAction action)
        {
            if (action == SQLAction.SELECT)
            {
                return queryCommands;
            }
            if (action == SQLAction.UPDATE)
            {
                return insertCommands;
            }
            else if (action == SQLAction.INSERT)
            {
                return updateCoommands;
            }
            else if (action == SQLAction.DELETE)
            {
                return deleteCommands;
            }

            return null;
        }
    }
}
