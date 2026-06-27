using Estoque.Application.DTOs;

namespace Estoque.Application.Services.Interfaces
{
    public interface IServicoService
    {
        Task<ServicoResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<ServicoResponseDTO>> ObterTodosAsync();
        Task<IEnumerable<ServicoResponseDTO>> ObterPorIdsAsync(IEnumerable<int> ids);
        Task<int> CriarAsync(ServicoRequestDTO dto);
        Task<bool> AtualizarAsync(int id, ServicoRequestDTO dto);
        Task<bool> ExcluirAsync(int id);
    }
}
