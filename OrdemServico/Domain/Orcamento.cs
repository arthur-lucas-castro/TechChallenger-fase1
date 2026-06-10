using Compartilhado.Domain;
using Compartilhado.Domain.ObjetosDeValor;

namespace OrdemServico.Domain
{
    public class Orcamento : EntidadeBase<Orcamento>
    {
        public int OrdemServicoId { get; set; }
        public int VendedorId { get; set; }
        public Dinheiro PrecoTotal { get; set; } = null!;
        public StatusOrcamento Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataEnvio { get; set; }
        public DateTime? DataAprovacao { get; set; }
    }
}
