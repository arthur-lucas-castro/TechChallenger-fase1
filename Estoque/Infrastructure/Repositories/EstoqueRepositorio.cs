using Compartilhado.Infrastructure.Repositories;
using Estoque.Domain.Interfaces;
using EstoqueEntidade = Estoque.Domain.Entities.Estoque;

namespace Estoque.Infrastructure.Repositories
{
    public class EstoqueRepositorio : BaseRepository<EstoqueEntidade>, IEstoqueRepositorio
    {
        public EstoqueRepositorio(AppDbContext context) : base(context) { }
    }
}
