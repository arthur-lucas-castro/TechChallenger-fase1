using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;

namespace Estoque.Domain.Entities
{
    public class Estoque : EntidadeBase<Estoque>, IAggregateRoot
    {
        public int PecaId { get; set; }
        public int QuantidadeAtual { get; set; }
        public int QuantidadeMinima { get; set; }
        public Dinheiro PrecoCustoMedio { get; set; } = null!;

        public Peca? Peca { get; set; }
    }
}
