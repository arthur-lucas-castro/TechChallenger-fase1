using Domain.Entidades.Base;
using Domain.ObjetosDeValor;

namespace Domain.Entidades
{
    public class TipoServico : EntidadeBase<TipoServico>
    {
        public string Nome { get; set; } = string.Empty;
        public Dinheiro PrecoVenda { get; set; } = null!;
        public int TempoEstimadoEmMinutos { get; set; }
    }
}

