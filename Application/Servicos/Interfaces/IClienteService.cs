using Application.Servicos.DTOs;

namespace Application.Servicos.Interfaces
{
    public interface IClienteService
    {
        Task<ClienteResponseDTO?> ObterPorIdAsync(int id);
        Task<IEnumerable<ClienteResponseDTO>> ObterTodosAsync();
        Task<int> CriarAsync(ClienteRequestDTO dto);
        Task<bool> AtualizarAsync(int id, ClienteRequestDTO dto);
        Task<bool> ExcluirAsync(int id);
    }
}
