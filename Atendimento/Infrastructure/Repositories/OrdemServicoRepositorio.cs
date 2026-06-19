using Atendimento.Domain.Entities;
using Atendimento.Domain.Interfaces;
using Compartilhado.Infrastructure.Repositories;

namespace Atendimento.Infrastructure.Repositories
{
    public class OrdemServicoRepositorio : BaseRepository<OrdemServico>, IOrdemServicoRepositorio
    {
        public OrdemServicoRepositorio(AppDbContext context) : base(context) { }
    }
}
