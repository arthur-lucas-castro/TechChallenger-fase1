using Compartilhado.Domain;
using Compartilhado.Domain.ObjetosDeValor;

namespace Estoque.Domain
{
    public class ItemServico : EntidadeBase<ItemServico>
    {
        public string Nome { get; set; } = string.Empty;
        public Dinheiro PrecoVenda { get; set; } = null!;
        public int TempoEstimadoEmMinutos { get; set; }
    }
}
