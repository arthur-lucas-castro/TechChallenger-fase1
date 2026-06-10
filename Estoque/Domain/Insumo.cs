using Compartilhado.Domain;
using Compartilhado.Domain.ObjetosDeValor;

namespace Estoque.Domain
{
    public class Insumo : EntidadeBase<Insumo>
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public Dinheiro Custo { get; set; } = null!;
        public Dinheiro PrecoVenda { get; set; } = null!;
    }
}
