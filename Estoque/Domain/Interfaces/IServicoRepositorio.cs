using Estoque.Domain.Entities;

namespace Estoque.Domain.Interfaces
{
    public interface IServicoRepositorio
    {
        Task<Servico?> GetByIdAsync(int id);
        Task<IEnumerable<Servico>> GetAllAsync();
        Task<int> InsertAsync(Servico servico);
        Task<bool> UpdateAsync(Servico servico);
        Task<bool> DeleteAsync(int id);
    }
}
