using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;

namespace Atendimento.Domain.Entities
{
    public class OrdemServico : EntidadeBase<OrdemServico>, IAggregateRoot
    {
        public int VeiculoId { get; set; }
        public int ClienteId { get; set; }
        public int ResponsavelId { get; set; }
        public StatusOrdemServico Status { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public ICollection<ServicoSolicitado> ServicosSolicitados { get; set; } = [];
        public ICollection<PecaSolicitada> PecasSolicitadas { get; set; } = [];
        public Orcamento? Orcamento { get; private set; }
        public Cliente? Cliente { get; set; }
        public Veiculo? Veiculo { get; set; }

        public void AlterarStatus(StatusOrdemServico novoStatus)
        {
            Status = novoStatus;
            DataUltimaAlteracao = DateTime.UtcNow;
        }

        public void FinalizarDiagnostico()
        {
            var totalServicos = ServicosSolicitados.Sum(s => s.PrecoVenda.Valor * s.Quantidade);
            var totalPecas = PecasSolicitadas.Sum(p => p.PrecoVenda.Valor * p.Quantidade);

            Orcamento = new Orcamento
            {
                OrdemServicoId = Id,
                PrecoTotal = new Dinheiro(totalServicos + totalPecas),
                Status = StatusOrcamento.Pendente,
                DataCriacao = DateTime.UtcNow
            };

            AlterarStatus(StatusOrdemServico.AguardandoAprovacao);
            AddDomainEvent(new Events.OrdemServicoDiagnosticoFinalizadoEvent(Id, ClienteId, VeiculoId));
        }

        public void FinalizarOrdem()
        {
            DataFinalizacao ??= DateTime.UtcNow;
            AlterarStatus(StatusOrdemServico.Finalizada);
        }

        public void EntregarVeiculo()
        {
            DataFinalizacao ??= DateTime.UtcNow;
            AlterarStatus(StatusOrdemServico.Entregue);
        }

        public void AlterarStatusServicoExecucao(int servicoSolicitadoId, StatusServicoExecucao novoStatus)
        {
            var servicoSolicitado = ServicosSolicitados.FirstOrDefault(s => s.Id == servicoSolicitadoId)
                ?? throw new InvalidOperationException($"Serviço solicitado {servicoSolicitadoId} não encontrado na ordem.");

            servicoSolicitado.ServicoExecucao ??= new ServicoExecucao
            {
                ServicoSolicitadoId = servicoSolicitadoId,
                Status = StatusServicoExecucao.Pendente
            };

            servicoSolicitado.ServicoExecucao.Status = novoStatus;

            if (novoStatus == StatusServicoExecucao.EmExecucao)
                servicoSolicitado.ServicoExecucao.DataInicio ??= DateTime.UtcNow;

            if (novoStatus == StatusServicoExecucao.Executado)
                servicoSolicitado.ServicoExecucao.DataFinalizacao ??= DateTime.UtcNow;
        }

        public void AdicionarServico(int servicoId, int quantidade, Dinheiro precoVenda)
        {
            ServicosSolicitados.Add(new ServicoSolicitado
            {
                ServicoId = servicoId,
                Quantidade = quantidade,
                PrecoVenda = precoVenda
            });
        }

        public bool RemoverServico(int servicoId)
        {
            var servico = ServicosSolicitados.FirstOrDefault(s => s.ServicoId == servicoId);
            if (servico is null) return false;
            ServicosSolicitados.Remove(servico);
            return true;
        }

        public void AdicionarPeca(int pecaId, string nome, int quantidade, Dinheiro precoVenda)
        {
            PecasSolicitadas.Add(new PecaSolicitada
            {
                PecaId = pecaId,
                Nome = nome,
                Quantidade = quantidade,
                PrecoVenda = precoVenda
            });
        }

        public bool RemoverPeca(int pecaId)
        {
            var peca = PecasSolicitadas.FirstOrDefault(p => p.PecaId == pecaId);
            if (peca is null) return false;
            PecasSolicitadas.Remove(peca);
            return true;
        }
    }
}
