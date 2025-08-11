using System.Data;

namespace SistemSekolahSMA.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}