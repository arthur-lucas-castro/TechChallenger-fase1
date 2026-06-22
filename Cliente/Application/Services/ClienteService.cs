using Cliente.Application.DTOs;
using Cliente.Application.Services.Interfaces;
using Cliente.Domain.Interfaces;
using Compartilhado.Domain.Entities;
using ClienteEntity = Cliente.Domain.Entities.Cliente;

namespace Cliente.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepositorio _repositorio;
        private readonly IDomainEventDispatcher _dispatcher;

        public ClienteService(IClienteRepositorio repositorio, IDomainEventDispatcher dispatcher)
        {
            _repositorio = repositorio;
            _dispatcher = dispatcher;
        }

        public async Task<ClienteResponseDTO?> ObterPorIdAsync(int id)
        {
            var c = await _repositorio.GetByIdAsync(id);
            return c is null ? null : MapearParaDTO(c);
        }

        public async Task<IEnumerable<ClienteResponseDTO>> ObterTodosAsync()
            => (await _repositorio.GetAllAsync()).Select(MapearParaDTO);

        public async Task<IEnumerable<ClienteResponseDTO>> ObterPorIdsAsync(IEnumerable<int> ids)
            => (await _repositorio.GetByExpressionAsync(c => ids.Contains(c.Id))).Select(MapearParaDTO);

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

        public async Task<bool> ResponderOrcamentoAsync(int clienteId, int ordemServicoId, ResponderOrcamentoDTO dto)
        {
            var cliente = await _repositorio.GetByIdAsync(clienteId);
            if (cliente is null) return false;

            cliente.ResponderOrcamento(ordemServicoId, dto.Aprovado);
            await _dispatcher.DispatchAsync(cliente.GetDomainEvents());
            cliente.ClearDomainEvents();
            return true;
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
