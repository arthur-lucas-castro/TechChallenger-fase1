using Catalogo.Application.DTOs;

namespace Catalogo.Application.Services.Interfaces
{
    public interface IPecaService
    {
        Task<PecaResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<PecaResponseDTO>> ObterTodosAsync();
        Task<int> CriarAsync(PecaRequestDTO dto);
        Task<bool> AtualizarAsync(int id, PecaRequestDTO dto);
        Task<bool> ExcluirAsync(int id);

        Task<IEnumerable<EstoqueComPecaResponseDTO>> ObterEstoqueTodosAsync();
        Task<EstoqueResponseDTO> AdicionarEstoqueAsync(EntradaEstoqueRequestDTO dto);
        Task<EstoqueResponseDTO?> DarBaixaAsync(BaixaEstoqueRequestDTO dto);
    }
}
