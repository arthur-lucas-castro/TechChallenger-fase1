using Cliente.Application.DTOs;
using Cliente.Application.Services.Interfaces;
using Cliente.Domain.Interfaces;
using ClienteEntity = Cliente.Domain.Entities.Cliente;

namespace Cliente.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepositorio _repositorio;

        public ClienteService(IClienteRepositorio repositorio) => _repositorio = repositorio;

        public async Task<ClienteResponseDTO?> ObterPorIdAsync(int id)
        {
            var c = await _repositorio.GetByIdAsync(id);
            return c is null ? null : MapearParaDTO(c);
        }

        public async Task<IEnumerable<ClienteResponseDTO>> ObterTodosAsync()
            => (await _repositorio.GetAllAsync()).Select(MapearParaDTO);

        public Task<int> CriarAsync(ClienteRequestDTO dto)
            => _repositorio.InsertAsync(CriarEntidade(dto));

        public async Task<bool> AtualizarAsync(int id, ClienteRequestDTO dto)
        {
            var e = CriarEntidade(dto);
            e.Id = id;
            return await _repositorio.UpdateAsync(e);
        }

        public Task<bool> ExcluirAsync(int id) => _repositorio.DeleteAsync(id);

        public async Task<ClienteResponseDTO?> ObterPorNumeroDocumentoAsync(string numeroDocumento)
        {
            var c = await _repositorio.ObterPorNumeroDocumentoAsync(numeroDocumento);
            return c is null ? null : MapearParaDTO(c);
        }

        private static ClienteResponseDTO MapearParaDTO(ClienteEntity c) => new()
        {
            Id = c.Id, Nome = c.Nome, Sobrenome = c.Sobrenome,
            Telefone = c.Telefone, Email = c.Email,
            NumeroDocumento = c.NumeroDocumento, TipoPessoa = c.TipoPessoa
        };

        private static ClienteEntity CriarEntidade(ClienteRequestDTO dto) => new()
        {
            Nome = dto.Nome, Sobrenome = dto.Sobrenome,
            Telefone = dto.Telefone, Email = dto.Email,
            NumeroDocumento = dto.NumeroDocumento, TipoPessoa = dto.TipoPessoa
        };
    }
}
