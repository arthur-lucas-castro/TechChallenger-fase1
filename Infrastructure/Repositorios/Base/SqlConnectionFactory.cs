using Npgsql;
using Microsoft.Extensions.Configuration;
using Infrastructure.Repositorios.Base.Interface;
using System.Data;

namespace Infrastructure.Repositorios.Base
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            //TODO? Obter via vault
            _connectionString = configuration.GetConnectionString("Default")!;
        }

        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }
}

