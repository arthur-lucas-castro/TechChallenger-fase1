using Cliente.Application.DTOs;

namespace Cliente.Application.Services.Interfaces
{
    public interface IVeiculoService
    {
        Task<VeiculoResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<VeiculoResponseDTO>> ObterTodosAsync();
        Task<int> CriarAsync(VeiculoRequestDTO dto);
        Task<bool> AtualizarAsync(int id, VeiculoRequestDTO dto);
        Task<bool> ExcluirAsync(int id);
        Task<VeiculoResponseDTO?> ObterPorPlacaAsync(string placa);
    }
}
