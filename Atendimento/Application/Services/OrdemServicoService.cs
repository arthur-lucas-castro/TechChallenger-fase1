using Atendimento.Application.DTOs;
using Atendimento.Application.Services.Interfaces;
using Atendimento.Domain.Entities;
using Atendimento.Domain.Interfaces;
using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;
using StatusServicoExecucao = Compartilhado.Domain.ValueObjects.StatusServicoExecucao;
using Estoque.Application.Services.Interfaces;
using Estoque.Application.DTOs;
using Cliente.Application.Services.Interfaces;

namespace Atendimento.Application.Services
{
    public class OrdemServicoService : IOrdemServicoService
    {
        private readonly IOrdemServicoRepositorio _repositorio;
        private readonly IServicoService _servicoService;
        private readonly IPecaService _pecaService;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly IClienteService _clienteService;
        private readonly IVeiculoService _veiculoService;

        public OrdemServicoService(IOrdemServicoRepositorio repositorio, IServicoService servicoService, IPecaService pecaService, IDomainEventDispatcher dispatcher, IClienteService clienteService, IVeiculoService veiculoService)
        {
            _repositorio = repositorio;
            _servicoService = servicoService;
            _pecaService = pecaService;
            _dispatcher = dispatcher;
            _clienteService = clienteService;
            _veiculoService = veiculoService;
        }

        public async Task<OrdemServicoResponseDTO?> ObterPorIdAsync(int id)
        {
            var os = await _repositorio.GetByIdAsync(id);
            return os is null ? null : MapearParaDTO(os);
        }

        public async Task<OrdemServicoDetalhadaResponseDTO?> ObterDetalhadoPorIdAsync(int id)
        {
            var os = await _repositorio.GetByIdDetalhadoAsync(id);
            if (os is null) return null;

            var cliente = await _clienteService.ObterPorIdAsync(os.ClienteId);
            var veiculo = await _veiculoService.ObterPorIdAsync(os.VeiculoId);

            var servicoIds = os.ServicosSolicitados.Select(s => s.ServicoId).Distinct();
            var nomesServico = new Dictionary<int, string>();
            foreach (var sid in servicoIds)
            {
                var servico = await _servicoService.ObterPorIdAsync(sid);
                if (servico is not null)
                    nomesServico[sid] = servico.Nome;
            }

            return new OrdemServicoDetalhadaResponseDTO
            {
                Id = os.Id,
                Status = os.Status.ToString(),
                DataCriacao = os.DataCriacao,
                DataUltimaAlteracao = os.DataUltimaAlteracao,
                DataFinalizacao = os.DataFinalizacao,

                ClienteId = os.ClienteId,
                NomeCliente = cliente?.Nome,
                SobrenomeCliente = cliente?.Sobrenome,
                TelefoneCliente = cliente?.Telefone,
                EmailCliente = cliente?.Email,
                DocumentoCliente = cliente?.NumeroDocumento,

                VeiculoId = os.VeiculoId,
                ModeloVeiculo = veiculo?.Modelo,
                MarcaVeiculo = veiculo?.Marca,
                AnoVeiculo = veiculo?.Ano,
                PlacaVeiculo = veiculo?.Placa,

                Servicos = os.ServicosSolicitados.Select(s => new ServicoSolicitadoResponseDTO
                {
                    Id = s.Id,
                    ServicoId = s.ServicoId,
                    NomeServico = nomesServico.GetValueOrDefault(s.ServicoId),
                    Quantidade = s.Quantidade,
                    PrecoVenda = s.PrecoVenda.Valor,
                    StatusExecucao = s.ServicoExecucao?.Status.ToString(),
                    DataInicioExecucao = s.ServicoExecucao?.DataInicio,
                    DataFinalizacaoExecucao = s.ServicoExecucao?.DataFinalizacao
                }).ToList(),

                Pecas = os.PecasSolicitadas.Select(p => new PecaSolicitadaResponseDTO
                {
                    Id = p.Id,
                    PecaId = p.PecaId,
                    Nome = p.Nome,
                    Quantidade = p.Quantidade,
                    PrecoVenda = p.PrecoVenda.Valor
                }).ToList(),

                Orcamento = os.Orcamento is null ? null : new OrcamentoResponseDTO
                {
                    Id = os.Orcamento.Id,
                    PrecoTotal = os.Orcamento.PrecoTotal.Valor,
                    Status = os.Orcamento.Status.ToString(),
                    DataCriacao = os.Orcamento.DataCriacao,
                    DataEnvio = os.Orcamento.DataEnvio,
                    DataAprovacao = os.Orcamento.DataAprovacao
                }
            };
        }

        public async Task<IEnumerable<OrdemServicoResponseDTO>> ObterTodosAsync()
        {
            var ordens = (await _repositorio.GetAllAsync()).ToList();

            var clienteIds = ordens.Select(o => o.ClienteId).Distinct();
            var clientes = (await _clienteService.ObterPorIdsAsync(clienteIds)).ToDictionary(c => c.Id);

            var veiculoIds = ordens.Select(o => o.VeiculoId).Distinct();
            var veiculos = (await _veiculoService.ObterPorIdsAsync(veiculoIds)).ToDictionary(v => v.Id);

            return ordens.Select(o =>
            {
                var dto = MapearParaDTO(o);
                if (clientes.TryGetValue(o.ClienteId, out var c))
                {
                    dto.NomeCliente = c.Nome;
                    dto.SobrenomeCliente = c.Sobrenome;
                }
                if (veiculos.TryGetValue(o.VeiculoId, out var v))
                {
                    dto.ModeloVeiculo = v.Modelo;
                    dto.MarcaVeiculo = v.Marca;
                    dto.AnoVeiculo = v.Ano;
                    dto.PlacaVeiculo = v.Placa;
                }
                return dto;
            });
        }

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

        public async Task<bool> AlterarStatusServicoExecucaoAsync(int ordemServicoId, int servicoSolicitadoId, AlterarStatusServicoExecucaoDTO dto)
        {
            var os = await _repositorio.GetByIdComServicosEExecucaoAsync(ordemServicoId);
            if (os is null) return false;

            os.AlterarStatusServicoExecucao(servicoSolicitadoId, Enum.Parse<StatusServicoExecucao>(dto.Status));
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
            Status = os.Status.ToString(),
            DataCriacao = os.DataCriacao,
            DataUltimaAlteracao = os.DataUltimaAlteracao,
            DataFinalizacao = os.DataFinalizacao
        };

        private static OrdemServico MapearParaEntidade(OrdemServicoRequestDTO dto) => new()
        {
            VeiculoId = dto.VeiculoId,
            ClienteId = dto.ClienteId,
            Status = StatusOrdemServico.Recebida,
            DataCriacao = DateTime.UtcNow
        };
    }
}
