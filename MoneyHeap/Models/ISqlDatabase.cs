using System.Data.SqlClient;

namespace MoneyHeap.Models
{
    public interface ISqlDatabase
    {
        public SqlConnection getSqlConnection();
    }

}
