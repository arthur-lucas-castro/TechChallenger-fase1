using Domain.Entidades.Base;
using Domain.ObjetosDeValor;

namespace Domain.Entidades
{
    public class ItemServico : EntidadeBase<ItemServico>
    {
        public string Nome { get; set; } = string.Empty;
        public Dinheiro PrecoVenda { get; set; } = null!;
        public int TempoEstimadoEmMinutos { get; set; }
    }
}
