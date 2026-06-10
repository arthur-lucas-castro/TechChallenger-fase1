using System.Data;

namespace Compartilhado.Infrastructure.Base.Interface
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
