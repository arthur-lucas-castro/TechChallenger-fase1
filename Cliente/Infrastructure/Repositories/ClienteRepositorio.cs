using Cliente.Domain.Interfaces;
using Cliente.Domain.ValueObjects;
using Compartilhado.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using ClienteEntity = Cliente.Domain.Entities.Cliente;

namespace Cliente.Infrastructure.Repositories
{
    public class ClienteRepositorio : BaseRepository<ClienteEntity>, IClienteRepositorio
    {
        public ClienteRepositorio(AppDbContext context) : base(context) { }

        public async Task<ClienteEntity?> ObterPorNumeroDocumentoAsync(string numeroDocumento)
        {
            Documento doc = numeroDocumento;
            return await _dbSet.FirstOrDefaultAsync(c => c.NumeroDocumento == doc);
        }
    }
}
