using Domain.Entidades.Base;

namespace Domain.Entidades
{
    public class OrdemServicoInsumo : EntidadeBase<OrdemServicoInsumo>
    {
        public int OrdemServicoId { get; set; }
        public int InsumoId { get; set; }
        public int Quantidade { get; set; }
    }
}

