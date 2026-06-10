using Cliente.Application.DTOs;

namespace Cliente.Application.Interfaces
{
    public interface IClienteService
    {
        Task<ClienteResponseDTO?> ObterPorIdAsync(int id);
        Task<ClienteResponseDTO?> ObterPorNumeroDocumentoAsync(string numeroDocumento);
        Task<IEnumerable<ClienteResponseDTO>> ObterTodosAsync();
        Task<int> CriarAsync(ClienteRequestDTO dto);
        Task<bool> AtualizarAsync(int id, ClienteRequestDTO dto);
        Task<bool> ExcluirAsync(int id);
    }
}
