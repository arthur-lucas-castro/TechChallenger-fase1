using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;
using Estoque.Domain.Entities.Events;

namespace Estoque.Domain.Entities
{
    public class ProdutoEstoque : EntidadeBase<ProdutoEstoque>
    {
        public int PecaId { get; set; }
        public int QuantidadeAtual { get; set; }
        public int QuantidadeMinima { get; set; }
        public Dinheiro PrecoCustoMedio { get; set; } = null!;


        public void DarBaixa(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade para baixa deve ser maior que zero.", nameof(quantidade));
            if (quantidade > QuantidadeAtual)
                throw new InvalidOperationException($"Estoque insuficiente. Disponível: {QuantidadeAtual}, solicitado: {quantidade}.");

            QuantidadeAtual -= quantidade;

        }
    }
}
