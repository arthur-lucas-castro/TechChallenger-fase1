using Atendimento.Domain.Entities;
using Atendimento.Domain.Interfaces;
using Compartilhado.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Atendimento.Infrastructure.Repositories
{
    public class OrdemServicoRepositorio : BaseRepository<OrdemServico>, IOrdemServicoRepositorio
    {
        public OrdemServicoRepositorio(AppDbContext context) : base(context) { }

        public async Task<OrdemServico?> GetByIdComServicosAsync(int id)
            => await _context.Set<OrdemServico>()
                .Include(o => o.ServicosSolicitados)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<OrdemServico?> GetByIdComPecasAsync(int id)
            => await _context.Set<OrdemServico>()
                .Include(o => o.PecasSolicitadas)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<OrdemServico?> GetByIdComItensAsync(int id)
            => await _context.Set<OrdemServico>()
                .Include(o => o.ServicosSolicitados)
                .Include(o => o.PecasSolicitadas)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<bool> CommitAsync()
            => await _context.SaveChangesAsync() > 0;
    }
}
