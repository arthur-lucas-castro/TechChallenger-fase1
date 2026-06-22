using Compartilhado.Domain.Entities;

namespace Cliente.Domain.Entities.Events
{
    public record OrcamentoRespondidoEvent(
        int ClienteId,
        int OrdemServicoId,
        bool Aprovado
    ) : IDomainEvent;
}
