using Repositorios.Base.Interface;
using System.Data;

namespace Repositorios.Base
{
    public abstract class ContextoBancoBase
    {
        private readonly IDbConnectionFactory _connectionFactory;

        protected ContextoBancoBase(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> action)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await action(connection);
        }

        protected async Task WithConnection(Func<IDbConnection, Task> action)
        {
            using var connection = _connectionFactory.CreateConnection();
            await action(connection);
        }
    }
}
