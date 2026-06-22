using Compartilhado.Domain.Entities;

namespace Atendimento.Domain.Entities.Events
{
    public record OrcamentoAprovadoEvent(
        int OrdemServicoId,
        int ClienteId,
        int OrcamentoId,
        decimal PrecoTotal
    ) : IDomainEvent;
}
