using Domain.Entidades.Base;
using Domain.ObjetosDeValor;

namespace Domain.Entidades
{
    public class OrdemServicoItem : EntidadeBase<OrdemServicoItem>
    {
        public int OrdemServicoId { get; set; }
        public int ItemServicoId { get; set; }
        public int Quantidade { get; set; }
        public Dinheiro Preco { get; set; } = null!;
    }
}

