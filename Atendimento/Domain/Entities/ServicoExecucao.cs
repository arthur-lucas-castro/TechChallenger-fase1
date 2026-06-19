using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;

namespace Atendimento.Domain.Entities
{
    public class ServicoExecucao : EntidadeBase<ServicoExecucao>
    {
        public int ServicoSolicitadoId { get; set; }
        public StatusServicoExecucao Status { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFinalizacao { get; set; }
    }
}
