using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Hong.DAO.Core
{
    public class SessionConnection
    {
        /// <summary>当前数据库连接
        /// </summary>
        DbConnection currentDbConnection = null;

        /// <summary>当前事务
        /// </summary>
        internal readonly SessionTransaction CurrentTransaction = new SessionTransaction();

        /// <summary>连接打开次数
        /// </summary>
        int openCount = 0;


        /// <summary>当前数据库连接
        /// </summary>
        internal DbConnection CurrentDbConnection
        {
            get
            {
                if (currentDbConnection != null)
                {
                    return currentDbConnection;
                }

                currentDbConnection = DataConfigurationManager.Configuration().CreateConnection();

                return currentDbConnection;
            }
        }

        /// <summary>打开连接
        /// </summary>
        public async Task Open()
        {
            if (openCount == 0 && CurrentDbConnection.State != System.Data.ConnectionState.Closed)
            {
                //防止意外连接未关闭情况,使用未关闭连接造成数据异常
                throw new Exception("先前的操作未关闭连接请检查代码");
            }

            if (CurrentDbConnection.State == System.Data.ConnectionState.Open)
            {
                System.Diagnostics.Debug.WriteLine("opened connnect,count:"+openCount);
                openCount++;
                return;
            }

            try
            {
                await CurrentDbConnection.OpenAsync();
            }
            catch
            {
                throw;
            }

            openCount++;
        }

        /// <summary>关闭连接
        /// </summary>
        public void Close()
        {
            if (openCount > 1)
            {
                openCount--;
                if (CurrentDbConnection.State == System.Data.ConnectionState.Closed)
                {
                    throw new Exception("中途存在连接未关闭问题");
                }

                return;
            }

            try
            {
                CurrentDbConnection.Close();
            }
            catch
            {
                SpinWait.SpinUntil(() => false, 10);

                if (CurrentTransaction.IsOpenedTransaction)
                {
                    try
                    {
                        Rollback();
                    }
                    catch { }
                }

                try
                {
                    CurrentDbConnection.Close();
                }
                catch
                {
                    //关闭失败释放连接,以防止数据异常
                    try
                    {
                        currentDbConnection.Dispose();
                    }
                    catch { }

                    currentDbConnection = null;
                }
            }

            openCount--;
        }

        /// <summary>开启事务
        /// </summary>
        public void BeginTran()
        {
            CurrentTransaction.BeginTran(CurrentDbConnection);
        }

        /// <summary>开启事务
        /// </summary>
        public void BeginTran(IsolationLevel isolationLevel)
        {
            CurrentTransaction.BeginTran(CurrentDbConnection, isolationLevel);
        }

        /// <summary>提交事务
        /// </summary>
        public void Commit()
        {
            CurrentTransaction.Commit();
        }

        /// <summary>回滚事务
        /// </summary>
        public void Rollback()
        {
            CurrentTransaction.Rollback();
        }
    }
}
