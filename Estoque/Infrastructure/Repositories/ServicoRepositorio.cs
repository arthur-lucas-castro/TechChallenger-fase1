using Compartilhado.Infrastructure.Repositories;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;

namespace Estoque.Infrastructure.Repositories
{
    public class ServicoRepositorio : BaseRepository<Servico>, IServicoRepositorio
    {
        public ServicoRepositorio(AppDbContext context) : base(context) { }
    }
}
