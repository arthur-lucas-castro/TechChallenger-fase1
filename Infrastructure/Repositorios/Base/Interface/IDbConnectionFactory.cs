using System.Data;

namespace Infrastructure.Repositorios.Base.Interface
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}

