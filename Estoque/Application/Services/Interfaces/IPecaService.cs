using Estoque.Application.DTOs;

namespace Estoque.Application.Services.Interfaces
{
    public interface IPecaService
    {
        Task<PecaResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<PecaResponseDTO>> ObterTodosAsync();
        Task<int> CriarAsync(PecaRequestDTO dto);
        Task<bool> AtualizarAsync(int id, PecaRequestDTO dto);
        Task<bool> ExcluirAsync(int id);
    }
}
