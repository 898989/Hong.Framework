using System.Data.Common;
using System.Threading.Tasks;

namespace Hong.DAO.Core
{
    public interface ISQLDAO
    {
        Task ExecuteNonQueryAsync(string cmdText, params object[] values);
        DbDataReader ExecuteReader(string cmdText, params object[] values);
        T ExecuteScalar<T>(string cmdText, params object[] values);
    }
}