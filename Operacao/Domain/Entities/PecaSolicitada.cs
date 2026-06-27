using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;

namespace Operacao.Domain.Entities
{
    public class PecaSolicitada : EntidadeBase<PecaSolicitada>
    {
        public int OrdemServicoId { get; set; }
        public int PecaId { get; set; }
        public int Quantidade { get; set; }
        public string Nome { get; set; } = string.Empty;
        public Dinheiro PrecoVenda { get; set; } = null!;
    }
}
