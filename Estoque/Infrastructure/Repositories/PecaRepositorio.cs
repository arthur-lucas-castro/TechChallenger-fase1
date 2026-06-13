using Compartilhado.Infrastructure.Repositories;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;

namespace Estoque.Infrastructure.Repositories
{
    public class PecaRepositorio : BaseRepository<Peca>, IPecaRepositorio
    {
        public PecaRepositorio(AppDbContext context) : base(context) { }
    }
}
