using Atendimento.Domain.Entities;

namespace Atendimento.Domain.Interfaces
{
    public interface IOrdemServicoRepositorio
    {
        Task<OrdemServico?> GetByIdAsync(int id);
        Task<IEnumerable<OrdemServico>> GetAllAsync();
        Task<int> InsertAsync(OrdemServico ordemServico);
        Task<bool> UpdateAsync(OrdemServico ordemServico);
        Task<bool> DeleteAsync(int id);
    }
}
