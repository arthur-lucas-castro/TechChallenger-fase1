using Estoque.Application.DTOs;

namespace Estoque.Application.Services.Interfaces
{
    public interface IEstoqueService
    {
        Task<IEnumerable<EstoqueComPecaResponseDTO>> ObterTodosAsync();
        Task<EstoqueResponseDTO> AdicionarProdutoAsync(EntradaEstoqueRequestDTO dto);
    }
}
