using Compartilhado.Domain.Entities;

namespace Atendimento.Domain.Entities.Events
{
    public record OrdemServicoDiagnosticoFinalizadoEvent(
        int OrdemServicoId,
        int ClienteId,
        int VeiculoId
    ) : IDomainEvent;
}
