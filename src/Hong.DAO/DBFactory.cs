using Hong.DAO.Core;

namespace Hong.DAO
{
    public class DBFactory<M>
    {
        private DBFactory()
        {
        }

        /// <summary>实体数据操作,优先选择此操作,不能实现才使用SQL操作
        /// </summary>
        public readonly IModelDAO<M> Model = new ModelDAO<M>();
        public readonly ISQLDAO SQL = new SQLDAO();

        /// <summary>创建事务管理
        /// </summary>
        /// <returns></returns>
        public TransactionScope CreateTransactionScope()
        {
            return new TransactionScope();
        }

        public static DBFactory<M> CreateInstance()
        {
            return new DBFactory<M>();
        }
    }
}
