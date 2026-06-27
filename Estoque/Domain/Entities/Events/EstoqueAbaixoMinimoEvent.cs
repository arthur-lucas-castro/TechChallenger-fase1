using Compartilhado.Domain.Entities;

namespace Estoque.Domain.Entities.Events
{
    public record EstoqueAbaixoMinimoEvent(
        int ProdutoId,
        string NomeProduto,
        int QuantidadeAtual,
        int QuantidadeMinima
    ) : IDomainEvent;
}
