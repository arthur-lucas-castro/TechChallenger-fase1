using Cliente.Domain.Entities;
using Cliente.Domain.Interfaces;
using Cliente.Domain.ValueObjects;
using Compartilhado.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cliente.Infrastructure.Repositories
{
    public class VeiculoRepositorio : BaseRepository<Veiculo>, IVeiculoRepositorio
    {
        public VeiculoRepositorio(AppDbContext context) : base(context) { }

        public async Task<Veiculo?> GetByPlacaAsync(string placa)
        {
            Placa placaVO = placa;
            return await _dbSet.FirstOrDefaultAsync(v => v.Placa == placaVO);
        }
    }
}
