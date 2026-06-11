using Compartilhado.Domain.Entities;

namespace OrdemServico.Domain.Entities
{
    public class OrdemServicoInsumo : EntidadeBase<OrdemServicoInsumo>
    {
        public int OrdemServicoId { get; set; }
        public int InsumoId { get; set; }
        public int Quantidade { get; set; }
    }
}
