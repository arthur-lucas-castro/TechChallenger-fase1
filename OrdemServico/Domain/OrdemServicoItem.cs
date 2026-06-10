using Compartilhado.Domain;
using Compartilhado.Domain.ObjetosDeValor;

namespace OrdemServico.Domain
{
    public class OrdemServicoItem : EntidadeBase<OrdemServicoItem>
    {
        public int OrdemServicoId { get; set; }
        public int ItemServicoId { get; set; }
        public int Quantidade { get; set; }
        public Dinheiro Preco { get; set; } = null!;
    }
}
