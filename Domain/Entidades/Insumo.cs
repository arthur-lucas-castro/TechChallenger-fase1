using Domain.Entidades.Base;
using Domain.ObjetosDeValor;

namespace Domain.Entidades
{
    public class Insumo : EntidadeBase<Insumo>
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public Dinheiro Custo { get; set; } = null!;
        public Dinheiro PrecoVenda { get; set; } = null!;
    }
}

