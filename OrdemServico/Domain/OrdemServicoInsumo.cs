using Compartilhado.Domain;

namespace OrdemServico.Domain
{
    public class OrdemServicoInsumo : EntidadeBase<OrdemServicoInsumo>
    {
        public int OrdemServicoId { get; set; }
        public int InsumoId { get; set; }
        public int Quantidade { get; set; }
    }
}
