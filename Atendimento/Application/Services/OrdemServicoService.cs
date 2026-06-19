using Atendimento.Application.DTOs;
using Atendimento.Application.Services.Interfaces;
using Atendimento.Domain.Entities;
using Atendimento.Domain.Interfaces;
using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;
using Estoque.Application.Services.Interfaces;
using Estoque.Application.DTOs;

namespace Atendimento.Application.Services
{
    public class OrdemServicoService : IOrdemServicoService
    {
        private readonly IOrdemServicoRepositorio _repositorio;
        private readonly IServicoService _servicoService;
        private readonly IPecaService _pecaService;
        private readonly IDomainEventDispatcher _dispatcher;

        public OrdemServicoService(IOrdemServicoRepositorio repositorio, IServicoService servicoService, IPecaService pecaService, IDomainEventDispatcher dispatcher)
        {
            _repositorio = repositorio;
            _servicoService = servicoService;
            _pecaService = pecaService;
            _dispatcher = dispatcher;
        }

        public async Task<OrdemServicoResponseDTO?> ObterPorIdAsync(int id)
        {
            var os = await _repositorio.GetByIdAsync(id);
            return os is null ? null : MapearParaDTO(os);
        }

        public async Task<IEnumerable<OrdemServicoResponseDTO>> ObterTodosAsync()
            => (await _repositorio.GetAllAsync()).Select(MapearParaDTO);

        public async Task<int> CriarAsync(OrdemServicoRequestDTO dto)
        {
            var ordemServico = MapearParaEntidade(dto);

            foreach (var item in dto.Servicos)
            {
                var servico = await _servicoService.ObterPorIdAsync(item.ServicoId)
                    ?? throw new KeyNotFoundException($"Serviço com Id {item.ServicoId} não encontrado.");

                ordemServico.AdicionarServico(item.ServicoId, item.Quantidade, (Dinheiro)servico.PrecoVenda);
            }

            foreach (var item in dto.Pecas)
            {
                var peca = await _pecaService.ObterPorIdAsync(item.PecaId)
                    ?? throw new KeyNotFoundException($"Peça com Id {item.PecaId} não encontrada.");

                ordemServico.AdicionarPeca(item.PecaId, peca.Nome, item.Quantidade, (Dinheiro)peca.PrecoVenda);
            }

            return await _repositorio.InsertAsync(ordemServico);
        }

        public async Task<bool> AtualizarAsync(int id, OrdemServicoRequestDTO dto)
        {
            var existente = await _repositorio.GetByIdAsync(id);
            if (existente is null) return false;
            existente.VeiculoId = dto.VeiculoId;
            existente.ClienteId = dto.ClienteId;
            existente.ResponsavelId = dto.ResponsavelId;
            existente.Status = Enum.Parse<StatusOrdemServico>(dto.Status);
            existente.DataUltimaAlteracao = DateTime.UtcNow;
            return await _repositorio.UpdateAsync(existente);
        }

        public Task<bool> ExcluirAsync(int id) => _repositorio.DeleteAsync(id);

        public async Task<bool> AdicionarServicoAsync(int ordemServicoId, ServicoSolicitadoRequestDTO dto)
        {
            var ordem = await _repositorio.GetByIdComServicosAsync(ordemServicoId);
            if (ordem is null) return false;

            var servico = await _servicoService.ObterPorIdAsync(dto.ServicoId)
                ?? throw new KeyNotFoundException($"Serviço com Id {dto.ServicoId} não encontrado.");

            ordem.AdicionarServico(dto.ServicoId, dto.Quantidade, (Dinheiro)servico.PrecoVenda);
            return await _repositorio.CommitAsync();
        }

        public async Task<bool> RemoverServicoAsync(int ordemServicoId, int servicoId)
        {
            var ordem = await _repositorio.GetByIdComServicosAsync(ordemServicoId);
            if (ordem is null) return false;
            if (!ordem.RemoverServico(servicoId)) return false;
            return await _repositorio.CommitAsync();
        }

        public async Task<bool> AdicionarPecaAsync(int ordemServicoId, PecaSolicitadaRequestDTO dto)
        {
            var ordem = await _repositorio.GetByIdComPecasAsync(ordemServicoId);
            if (ordem is null) return false;

            var peca = await _pecaService.ObterPorIdAsync(dto.PecaId)
                ?? throw new KeyNotFoundException($"Peça com Id {dto.PecaId} não encontrada.");

            ordem.AdicionarPeca(dto.PecaId, peca.Nome, dto.Quantidade, (Dinheiro)peca.PrecoVenda);
            return await _repositorio.CommitAsync();
        }

        public async Task<bool> RemoverPecaAsync(int ordemServicoId, int pecaId)
        {
            var ordem = await _repositorio.GetByIdComPecasAsync(ordemServicoId);
            if (ordem is null) return false;
            if (!ordem.RemoverPeca(pecaId)) return false;
            return await _repositorio.CommitAsync();
        }

        public async Task<bool> AlterarStatusAsync(int id, AlterarStatusOrdemServicoDTO dto)
        {
            var os = await _repositorio.GetByIdComItensAsync(id);
            if (os is null) return false;

            AplicarTransicaoStatus(os, Enum.Parse<StatusOrdemServico>(dto.Status));

            var resultado = await _repositorio.CommitAsync();
            await _dispatcher.DispatchAsync(os.GetDomainEvents());
            os.ClearDomainEvents();
            return resultado;
        }

        private static void AplicarTransicaoStatus(OrdemServico os, StatusOrdemServico status)
        {
            switch (status)
            {
                case StatusOrdemServico.AguardandoAprovacao: 
                    os.FinalizarDiagnostico(); 
                    break;
                case StatusOrdemServico.Finalizada:          
                    os.FinalizarOrdem();       
                    break;
                case StatusOrdemServico.Entregue:            
                    os.EntregarVeiculo();      
                    break;
                default:                                     
                    os.AlterarStatus(status);  
                    break;
            }
        }

        private static OrdemServicoResponseDTO MapearParaDTO(OrdemServico os) => new()
        {
            Id = ((Compartilhado.Domain.Entities.EntidadeBase<OrdemServico>)os).Id,
            VeiculoId = os.VeiculoId,
            ClienteId = os.ClienteId,
            ResponsavelId = os.ResponsavelId,
            Status = os.Status.ToString(),
            DataCriacao = os.DataCriacao,
            DataUltimaAlteracao = os.DataUltimaAlteracao,
            DataFinalizacao = os.DataFinalizacao
        };

        private static OrdemServico MapearParaEntidade(OrdemServicoRequestDTO dto) => new()
        {
            VeiculoId = dto.VeiculoId,
            ClienteId = dto.ClienteId,
            ResponsavelId = dto.ResponsavelId,
            Status = StatusOrdemServico.Recebida,
            DataCriacao = DateTime.UtcNow
        };
    }
}
