using Npgsql;
using Microsoft.Extensions.Configuration;
using Compartilhado.Infrastructure.Base.Interface;
using System.Data;

namespace Compartilhado.Infrastructure.Base
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")!;
        }

        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }
}
