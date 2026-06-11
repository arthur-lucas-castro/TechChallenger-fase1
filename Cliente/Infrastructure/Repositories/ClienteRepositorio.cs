using Dapper;
using Cliente.Domain.Interfaces;
using Compartilhado.Infrastructure.Repositories;
using Compartilhado.Infrastructure.Repositories.Interface;
using ClienteEntity = Cliente.Domain.Entities.Cliente;

namespace Cliente.Infrastructure.Repositories
{
    public class ClienteRepositorio : RepositorioBase<ClienteEntity>, IClienteRepositorio
    {
        public ClienteRepositorio(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<ClienteEntity?> ObterPorNumeroDocumentoAsync(string numeroDocumento)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<ClienteEntity>(
                $"SELECT * FROM {_tableName} WHERE NumeroDocumento = @NumeroDocumento",
                new { NumeroDocumento = numeroDocumento });
        }
    }
}
