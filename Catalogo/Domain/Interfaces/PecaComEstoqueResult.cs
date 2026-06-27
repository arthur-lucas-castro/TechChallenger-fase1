namespace Catalogo.Domain.Interfaces
{
    public record PecaComEstoqueResult(
        int PecaId,
        string Nome,
        string Descricao,
        decimal PrecoVenda,
        int? EstoqueId,
        int? QuantidadeAtual,
        int? QuantidadeMinima,
        decimal? PrecoCustoMedio
    );
}
