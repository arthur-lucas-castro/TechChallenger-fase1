using System.Linq.Expressions;
using Estoque.Domain.Entities;

namespace Estoque.Domain.Interfaces
{
    public interface IPecaRepositorio
    {
        Task<Peca?> GetByIdAsync(int id);
        Task<IEnumerable<Peca>> GetAllAsync();
        Task<IEnumerable<Peca>> GetByExpressionAsync(Expression<Func<Peca, bool>> predicate);
        Task<int> InsertAsync(Peca peca);
        Task<bool> UpdateAsync(Peca peca);
        Task<bool> DeleteAsync(int id);

        Task<Peca?> GetByIdComEstoqueAsync(int pecaId);
        Task<IEnumerable<Peca>> GetAllComEstoqueAsync();
        Task<int> InsertProdutoEstoqueAsync(ProdutoEstoque produtoEstoque);
        Task<bool> UpdateProdutoEstoqueAsync(ProdutoEstoque produtoEstoque);
    }
}
