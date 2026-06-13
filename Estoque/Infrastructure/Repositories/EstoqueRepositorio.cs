using Compartilhado.Infrastructure.Repositories;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using EstoqueEntidade = Estoque.Domain.Entities.Estoque;

namespace Estoque.Infrastructure.Repositories
{
    public class EstoqueRepositorio : BaseRepository<EstoqueEntidade>, IEstoqueRepositorio
    {
        public EstoqueRepositorio(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<EstoqueEntidade>> GetAllComPecaAsync()
            => await _dbSet.Include(e => e.Peca).ToListAsync();

        public async Task<IEnumerable<PecaComEstoqueResult>> GetPecasComEstoqueAsync()
            => await (
                from p in _context.Set<Peca>()
                join e in _context.Set<EstoqueEntidade>() on p.Id equals e.PecaId into estoques
                from estoque in estoques.DefaultIfEmpty()
                select new PecaComEstoqueResult(
                    p.Id,
                    p.Nome,
                    p.Descricao,
                    (decimal)p.PrecoVenda,
                    estoque != null ? estoque.Id : (int?)null,
                    estoque != null ? estoque.QuantidadeAtual : (int?)null,
                    estoque != null ? estoque.QuantidadeMinima : (int?)null,
                    estoque != null ? (decimal)estoque.PrecoCustoMedio : (decimal?)null
                )
            ).ToListAsync();
    }
}
