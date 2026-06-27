using Atendimento.Application.DTOs;

namespace Atendimento.Application.Services.Interfaces
{
    public interface IOrdemServicoService
    {
        Task<OrdemServicoResponseDTO?> ObterPorIdAsync(int id);
        Task<OrdemServicoDetalhadaResponseDTO?> ObterDetalhadoPorIdAsync(int id);
        Task<IEnumerable<OrdemServicoResponseDTO>> ObterTodosAsync();
        Task<int> CriarAsync(OrdemServicoRequestDTO dto);
        Task<bool> ExcluirAsync(int id);
        Task<bool> AlterarStatusAsync(int id, AlterarStatusOrdemServicoDTO dto);
        Task<bool> AdicionarServicoAsync(int ordemServicoId, ServicoSolicitadoRequestDTO dto);
        Task<bool> RemoverServicoAsync(int ordemServicoId, int servicoId);
        Task<bool> AdicionarPecaAsync(int ordemServicoId, PecaSolicitadaRequestDTO dto);
        Task<bool> RemoverPecaAsync(int ordemServicoId, int pecaId);
        Task<bool> AlterarStatusServicoExecucaoAsync(int ordemServicoId, int servicoSolicitadoId, AlterarStatusServicoExecucaoDTO dto);
        Task<bool> IniciarDiagnosticoAsync(int id);
        Task<bool> FinalizarDiagnosticoAsync(int id);
        Task<bool> IniciarServicoAsync(int ordemServicoId, int servicoSolicitadoId);
        Task<bool> FinalizarServicoAsync(int ordemServicoId, int servicoSolicitadoId);
        Task<IEnumerable<TempoExecucaoServicoResponseDTO>> ObterTemposExecucaoPorServicoAsync();
    }
}
