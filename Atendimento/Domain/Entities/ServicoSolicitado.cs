using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;

namespace Atendimento.Domain.Entities
{
    public class ServicoSolicitado : EntidadeBase<ServicoSolicitado>
    {
        public int OrdemServicoId { get; set; }
        public int ServicoId { get; set; }
        public int Quantidade { get; set; }
        public Dinheiro PrecoVenda { get; set; } = null!;
        public ServicoExecucao? ServicoExecucao { get; set; }
    }
}
