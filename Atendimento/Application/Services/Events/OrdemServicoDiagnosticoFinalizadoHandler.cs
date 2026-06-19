using Atendimento.Domain.Entities.Events;
using Compartilhado.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Atendimento.Application.Services.Events
{
    public class OrdemServicoDiagnosticoFinalizadoHandler : IDomainEventHandler<OrdemServicoDiagnosticoFinalizadoEvent>
    {
        private readonly ILogger<OrdemServicoDiagnosticoFinalizadoHandler> _logger;

        public OrdemServicoDiagnosticoFinalizadoHandler(ILogger<OrdemServicoDiagnosticoFinalizadoHandler> logger)
            => _logger = logger;

        public Task HandleAsync(OrdemServicoDiagnosticoFinalizadoEvent domainEvent, CancellationToken ct = default)
        {
            _logger.LogInformation(
                "Diagnóstico finalizado: ordem de serviço {OrdemServicoId}, cliente {ClienteId}, veículo {VeiculoId}.",
                domainEvent.OrdemServicoId,
                domainEvent.ClienteId,
                domainEvent.VeiculoId);

            return Task.CompletedTask;
        }
    }
}
