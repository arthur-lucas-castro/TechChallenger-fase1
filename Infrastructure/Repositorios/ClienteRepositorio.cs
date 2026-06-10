using Dapper;
using Domain.Entidades;
using Domain.Interfaces;
using Infrastructure.Repositorios.Base;
using Infrastructure.Repositorios.Base.Interface;

namespace Infrastructure.Repositorios
{
    public class ClienteRepositorio : RepositorioBase<Cliente>, IClienteRepositorio
    {
        public ClienteRepositorio(IDbConnectionFactory connectionFactory)
            : base(connectionFactory) { }

        public async Task<Cliente?> ObterPorNumeroDocumentoAsync(string numeroDocumento)
        {
            using var connection = this._connectionFactory.CreateConnection();

            var sql = $"SELECT * FROM {_tableName} WHERE NumeroDocumento = @NumeroDocumento";

            return await connection.QueryFirstOrDefaultAsync<Cliente>(sql, new { NumeroDocumento = numeroDocumento });
        }
    }
}
