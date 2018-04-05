using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Hong.DAO.Core
{
    /// <summary>
    /// 请求域事务
    /// </summary>
    public class SessionTransaction
    {
        DbTransaction transaction = null;

        /// <summary>事务层次
        /// </summary>
        int level = 0;

        /// <summary>是否开启事务
        /// </summary>
        internal bool IsOpenedTransaction = false;

        internal void BeginTran(DbConnection conn)
        {
            if (IsOpenedTransaction)
            {
                level++;
                return;
            }

            transaction = conn.BeginTransaction();
            IsOpenedTransaction = true;
            level++;
        }

        internal void BeginTran(DbConnection conn, IsolationLevel isolationLevel)
        {
            if (IsOpenedTransaction)
            {
                level++;
                return;
            }

            transaction = conn.BeginTransaction(isolationLevel);
            IsOpenedTransaction = true;
            level++;
        }

        /// <summary>提交事务
        /// </summary>
        public void Commit()
        {
            if (!IsOpenedTransaction)
            {
                return;
            }

            level--;

            if (level > 0)
            {
                return;
            }

            IsOpenedTransaction = false;
            transaction.Commit();
            transaction.Dispose();

            foreach (var eventItem in Events)
            {
                eventItem.Event(this, eventItem.ID);
            }
        }

        /// <summary>回滚事务
        /// </summary>
        public void Rollback()
        {
            level = 0;
            IsOpenedTransaction = false;
            Events.Clear();
            transaction.Rollback();
            transaction.Dispose();
        }

        /// <summary>事务完成引发的事件列表
        /// </summary>
        internal List<EventItem> Events = new List<EventItem>();
    }
}
