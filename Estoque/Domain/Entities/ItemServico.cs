using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;

namespace Estoque.Domain.Entities
{
    public class ItemServico : EntidadeBase<ItemServico>
    {
        public string Nome { get; set; } = string.Empty;
        public Dinheiro PrecoVenda { get; set; } = null!;
        public int TempoEstimadoEmMinutos { get; set; }
    }
}
