using Cliente.Application.DTOs;

namespace Cliente.Application.Services.Interfaces
{
    public interface IClienteService
    {
        Task<ClienteResponseDTO?> ObterPorIdAsync(int id);
        Task<ClienteResponseDTO?> ObterPorNumeroDocumentoAsync(string numeroDocumento);
        Task<IEnumerable<ClienteResponseDTO>> ObterTodosAsync();
        Task<IEnumerable<ClienteResponseDTO>> ObterPorIdsAsync(IEnumerable<int> ids);
        Task<int> CriarAsync(ClienteRequestDTO dto);
        Task<bool> AtualizarAsync(int id, ClienteRequestDTO dto);
        Task<bool> ExcluirAsync(int id);
        Task<bool> ResponderOrcamentoAsync(int clienteId, int ordemServicoId, ResponderOrcamentoDTO dto);
    }
}
