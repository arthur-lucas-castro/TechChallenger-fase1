using System.Linq.Expressions;
using EstoqueEntidade = Estoque.Domain.Entities.Estoque;

namespace Estoque.Domain.Interfaces
{
    public interface IEstoqueRepositorio
    {
        Task<EstoqueEntidade?> GetByIdAsync(int id);
        Task<IEnumerable<EstoqueEntidade>> GetAllAsync();
        Task<IEnumerable<EstoqueEntidade>> GetAllComPecaAsync();
        Task<IEnumerable<PecaComEstoqueResult>> GetPecasComEstoqueAsync();
        Task<IEnumerable<EstoqueEntidade>> GetByExpressionAsync(Expression<Func<EstoqueEntidade, bool>> predicate);
        Task<int> InsertAsync(EstoqueEntidade estoque);
        Task<bool> UpdateAsync(EstoqueEntidade estoque);
        Task<bool> DeleteAsync(int id);
    }
}
