using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Repositorios.Base.Interface;
using System.Data;

namespace Repositorios.Base
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            //TODO? Obter via vault
            _connectionString = configuration.GetConnectionString("Default");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
