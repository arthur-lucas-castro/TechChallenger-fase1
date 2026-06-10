using Estoque.Application.DTOs;

namespace Estoque.Application.Interfaces
{
    public interface IItemServicoService
    {
        Task<ItemServicoResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<ItemServicoResponseDTO>> ObterTodosAsync();
        Task<int> CriarAsync(ItemServicoRequestDTO dto);
        Task<bool> AtualizarAsync(int id, ItemServicoRequestDTO dto);
        Task<bool> ExcluirAsync(int id);
    }
}
