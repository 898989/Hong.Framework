using Hong.DAO.Core;
using System;
using System.Data;

namespace Hong.DAO
{
    /// <summary>范围内事务
    /// </summary>
    public class TransactionScope : IDisposable
    {
        private bool isCommit = false;
        private bool isDisposed = false;
        SessionConnection conn = null;

        internal TransactionScope()
        {
            conn = DataBase.Connection;

            conn.Open().Wait();
            conn.BeginTran();
        }

        internal TransactionScope(IsolationLevel isolationLevel)
        {
            conn = DataBase.Connection;

            conn.Open().Wait();
            conn.BeginTran(isolationLevel);
        }

        /// <summary>执行完成
        /// </summary>
        public void Complete()
        {
            conn.Commit();
            isCommit = true;

            conn.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool diposing)
        {
            if (!isDisposed && !isCommit)
            {
                try
                {
                    conn.Rollback();
                }
                catch { }
                finally
                {
                    conn.Close();
                }
            }

            isDisposed = true;
        }

        ~TransactionScope()
        {
            Dispose(false);
        }
    }
}
