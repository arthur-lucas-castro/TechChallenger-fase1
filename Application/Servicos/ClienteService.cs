using Application.Servicos.DTOs;
using Application.Servicos.Interfaces;
using Domain.Entidades;
using Domain.Interfaces;

namespace Application.Servicos
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepositorio _repositorio;

        public ClienteService(IClienteRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<ClienteResponseDTO?> ObterPorIdAsync(int id)
        {
            var cliente = await _repositorio.GetByIdAsync(id);
            return cliente is null ? null : MapearParaDTO(cliente);
        }

        public async Task<IEnumerable<ClienteResponseDTO>> ObterTodosAsync()
        {
            var clientes = await _repositorio.GetAllAsync();
            return clientes.Select(MapearParaDTO);
        }

        public Task<int> CriarAsync(ClienteRequestDTO dto)
        {
            var cliente = CriarEntidade(dto);
            return _repositorio.InsertAsync(cliente);
        }

        public async Task<bool> AtualizarAsync(int id, ClienteRequestDTO dto)
        {
            var entidade = CriarEntidade(dto);
            entidade.Id = id;
            return await _repositorio.UpdateAsync(entidade);
        }

        public Task<bool> ExcluirAsync(int id)
            => _repositorio.DeleteAsync(id);

        private static ClienteResponseDTO MapearParaDTO(Cliente c) => new()
        {
            Id              = c.Id,
            Nome            = c.Nome,
            Sobrenome       = c.Sobrenome,
            Telefone        = c.Telefone,
            Email           = c.Email,
            NumeroDocumento = c.NumeroDocumento,
            TipoPessoa      = c.TipoPessoa
        };

        private static Cliente CriarEntidade(ClienteRequestDTO dto) => new()
        {
            Nome            = dto.Nome,
            Sobrenome       = dto.Sobrenome,
            Telefone        = dto.Telefone,
            Email           = dto.Email,
            NumeroDocumento = dto.NumeroDocumento,
            TipoPessoa      = dto.TipoPessoa
        };

        public async Task<ClienteResponseDTO?> ObterPorNumeroDocumentoAsync(string numeroDocumento)
        {
            var cliente = await _repositorio.ObterPorNumeroDocumentoAsync(numeroDocumento);
            return cliente is null ? null : MapearParaDTO(cliente);
        }
    }
}
