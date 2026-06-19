using Atendimento.Application.DTOs;

namespace Atendimento.Application.Services.Interfaces
{
    public interface IOrdemServicoService
    {
        Task<OrdemServicoResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<OrdemServicoResponseDTO>> ObterTodosAsync();
        Task<int> CriarAsync(OrdemServicoRequestDTO dto);
        Task<bool> AtualizarAsync(int id, OrdemServicoRequestDTO dto);
        Task<bool> ExcluirAsync(int id);
        Task<bool> AlterarStatusAsync(int id, AlterarStatusOrdemServicoDTO dto);
        Task<bool> AdicionarServicoAsync(int ordemServicoId, ServicoSolicitadoRequestDTO dto);
        Task<bool> RemoverServicoAsync(int ordemServicoId, int servicoId);
        Task<bool> AdicionarPecaAsync(int ordemServicoId, PecaSolicitadaRequestDTO dto);
        Task<bool> RemoverPecaAsync(int ordemServicoId, int pecaId);
    }
}
