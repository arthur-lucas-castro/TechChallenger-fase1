using Dapper;
using Cliente.Domain;
using Compartilhado.Infrastructure.Base;
using Compartilhado.Infrastructure.Base.Interface;

namespace Cliente.Infrastructure
{
    public class ClienteRepositorio : RepositorioBase<Domain.Cliente>, IClienteRepositorio
    {
        public ClienteRepositorio(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<Domain.Cliente?> ObterPorNumeroDocumentoAsync(string numeroDocumento)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Domain.Cliente>(
                $"SELECT * FROM {_tableName} WHERE NumeroDocumento = @NumeroDocumento",
                new { NumeroDocumento = numeroDocumento });
        }
    }
}
