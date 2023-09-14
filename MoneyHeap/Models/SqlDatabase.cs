
using System.Data.SqlClient;

namespace MoneyHeap.Models
{
    public class SqlDatabase : ISqlDatabase
    {
        public SqlConnection connection = null;
        public SqlConnection getSqlConnection()
        {
            if (connection == null)
            {
                connection = new SqlConnection();
            }
                return connection;
        }
    }
}
