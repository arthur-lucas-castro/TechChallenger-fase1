using Atendimento.Application.DTOs;

namespace Atendimento.Application.Services.Interfaces
{
    public interface IVeiculoService
    {
        Task<VeiculoResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<VeiculoResponseDTO>> ObterTodosAsync();
        Task<IEnumerable<VeiculoResponseDTO>> ObterPorIdsAsync(IEnumerable<int> ids);
        Task<int> CriarAsync(VeiculoRequestDTO dto);
        Task<bool> AtualizarAsync(int id, VeiculoRequestDTO dto);
        Task<bool> ExcluirAsync(int id);
        Task<VeiculoResponseDTO?> ObterPorPlacaAsync(string placa);
    }
}
