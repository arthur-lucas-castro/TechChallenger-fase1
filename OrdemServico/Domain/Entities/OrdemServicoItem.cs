using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;

namespace OrdemServico.Domain.Entities
{
    public class OrdemServicoItem : EntidadeBase<OrdemServicoItem>
    {
        public int OrdemServicoId { get; set; }
        public int ServicoId { get; set; }
        public int Quantidade { get; set; }
        public Dinheiro Preco { get; set; } = null!;
    }
}
