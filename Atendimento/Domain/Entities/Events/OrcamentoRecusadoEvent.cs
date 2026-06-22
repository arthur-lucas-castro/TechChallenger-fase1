using Compartilhado.Domain.Entities;

namespace Atendimento.Domain.Entities.Events
{
    public record OrcamentoRecusadoEvent(
        int OrdemServicoId,
        int ClienteId,
        int OrcamentoId
    ) : IDomainEvent;
}
