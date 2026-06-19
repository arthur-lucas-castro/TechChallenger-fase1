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
        Task<OrdemServico?> GetByIdComServicosAsync(int id);
        Task<OrdemServico?> GetByIdComPecasAsync(int id);
        Task<OrdemServico?> GetByIdComItensAsync(int id);
        Task<OrdemServico?> GetByIdComServicosEExecucaoAsync(int id);
        Task<bool> CommitAsync();
    }
}
