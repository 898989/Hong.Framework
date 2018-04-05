using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Threading.Tasks;
using static Hong.Common.Extendsion.Guard;

namespace Hong.DAO.Core
{
    public class SQLDAO : DataBase, ISQLDAO
    {
        internal SQLDAO(ILoggerFactory loggerFactory = null) : base(loggerFactory)
        {
        }

        /// <summary>SQL命令仓库
        /// </summary>
        private CmdExcuteRepository cmdExcuteRepository = new CmdExcuteRepository();


        /// <summary>执行SQL语句
        /// </summary>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="values">参数</param>
        public async Task ExecuteNonQueryAsync(string cmdText, params object[] values)
        {
            NotNullOrEmpty(cmdText, nameof(cmdText));

            var repository = cmdExcuteRepository.GetRepository(cmdText);
            var cmd = repository.Pop(Connection);
            cmd.CommandText = cmdText;

            if (values != null)
            {
                if (cmd.Parameters.Count == 0)
                {
                    AddGeneralParams(cmd, values);
                }
                else
                {
                    int count = cmd.Parameters.Count;
                    var ps = cmd.Parameters;
                    for (var i = 0; i < count; i++)
                    {
                        ps[i].Value = values[i];
                    }
                }
            }

            try
            {
                await ExecuteNonQuery(cmd);
            }
            finally
            {
                repository.Push(cmd);
            }
        }

        /// <summary>执行SQL语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="values">参数</param>
        /// <returns>返回第一行第一列值</returns>
        public T ExecuteScalar<T>(string cmdText, params object[] values)
        {
            NotNullOrEmpty(cmdText, nameof(cmdText));

            var repository = cmdExcuteRepository.GetRepository(cmdText);
            var cmd = repository.Pop(Connection);
            cmd.CommandText = cmdText;

            if (values != null)
            {
                if (cmd.Parameters.Count == 0)
                {
                    AddGeneralParams(cmd, values);
                }
                else
                {
                    int count = cmd.Parameters.Count;
                    var ps = cmd.Parameters;
                    for (var i = 0; i < count; i++)
                    {
                        ps[i].Value = values[i];
                    }
                }
            }

            object obj = null;

            try
            {
                obj = ExecuteScalar(cmd).Result;
            }
            finally
            {
                repository.Push(cmd);
            }

            if (obj == null)
            {
                return default(T);
            }

            return (T)obj;
        }

        /// <summary>执行SQL语句
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string cmdText, params object[] values)
        {
            NotNullOrEmpty(cmdText, nameof(cmdText));

            var repository = cmdExcuteRepository.GetRepository(cmdText);
            var cmd = repository.Pop(Connection);
            cmd.CommandText = cmdText;

            if (values != null)
            {
                if (cmd.Parameters.Count == 0)
                {
                    AddGeneralParams(cmd, values);
                }
                else
                {
                    int count = cmd.Parameters.Count;
                    var ps = cmd.Parameters;
                    for (var i = 0; i < count; i++)
                    {
                        ps[i].Value = values[i];
                    }
                }
            }

            try
            {
                return cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection).Result;
            }
            finally
            {
                repository.Push(cmd);
            }
        }
    }
}
