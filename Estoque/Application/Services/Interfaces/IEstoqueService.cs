using Estoque.Application.DTOs;

namespace Estoque.Application.Services.Interfaces
{
    public interface IEstoqueService
    {
        Task<EstoqueResponseDTO> AdicionarProdutoAsync(EntradaEstoqueRequestDTO dto);
    }
}
