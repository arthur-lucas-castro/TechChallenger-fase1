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

        public void AlterarStatus(StatusOrdemServico novoStatus)
        {
            Status = novoStatus;
            DataUltimaAlteracao = DateTime.UtcNow;

            if (novoStatus == StatusOrdemServico.Finalizada || novoStatus == StatusOrdemServico.Entregue)
                DataFinalizacao ??= DateTime.UtcNow;
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
    }
}
