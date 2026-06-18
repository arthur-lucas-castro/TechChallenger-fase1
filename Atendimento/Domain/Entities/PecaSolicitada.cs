using Compartilhado.Domain.Entities;

namespace Atendimento.Domain.Entities
{
    public class PecaSolicitada : EntidadeBase<PecaSolicitada>
    {
        public int OrdemServicoId { get; set; }
        public int PecaId { get; set; }
        public int Quantidade { get; set; }
    }
}
