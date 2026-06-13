using Estoque.Application.DTOs;
using Estoque.Application.Services.Interfaces;
using Estoque.Domain.Interfaces;
using EstoqueEntidade = Estoque.Domain.Entities.Estoque;

namespace Estoque.Application.Services
{
    public class EstoqueService : IEstoqueService
    {
        private readonly IEstoqueRepositorio _repositorio;

        public EstoqueService(IEstoqueRepositorio repositorio) => _repositorio = repositorio;

        public async Task<IEnumerable<EstoqueComPecaResponseDTO>> ObterTodosAsync()
        {
            var resultado = await _repositorio.GetPecasComEstoqueAsync();
            return resultado.Select(r => new EstoqueComPecaResponseDTO
            {
                PecaId           = r.PecaId,
                NomePeca         = r.Nome,
                DescricaoPeca    = r.Descricao,
                PrecoVendaPeca   = r.PrecoVenda,
                EstoqueId        = r.EstoqueId,
                QuantidadeAtual  = r.QuantidadeAtual,
                QuantidadeMinima = r.QuantidadeMinima,
                PrecoCustoMedio  = r.PrecoCustoMedio
            });
        }

        public async Task<EstoqueResponseDTO> AdicionarProdutoAsync(EntradaEstoqueRequestDTO dto)
        {
            var registros = await _repositorio.GetByExpressionAsync(e => e.PecaId == dto.PecaId);
            var estoque = registros.FirstOrDefault();

            if (estoque is null)
            {
                estoque = new EstoqueEntidade
                {
                    PecaId           = dto.PecaId,
                    QuantidadeAtual  = dto.Quantidade,
                    QuantidadeMinima = 0,
                    PrecoCustoMedio  = dto.PrecoCusto
                };
                estoque.Id = await _repositorio.InsertAsync(estoque);
            }
            else
            {
                var novoPrecoMedio = (estoque.QuantidadeAtual * (decimal)estoque.PrecoCustoMedio
                                     + dto.Quantidade * dto.PrecoCusto)
                                    / (estoque.QuantidadeAtual + dto.Quantidade);

                estoque.QuantidadeAtual  += dto.Quantidade;
                estoque.PrecoCustoMedio   = Math.Round(novoPrecoMedio, 2);
                await _repositorio.UpdateAsync(estoque);
            }

            return new EstoqueResponseDTO
            {
                Id               = estoque.Id,
                PecaId           = estoque.PecaId,
                QuantidadeAtual  = estoque.QuantidadeAtual,
                QuantidadeMinima = estoque.QuantidadeMinima,
                PrecoCustoMedio  = estoque.PrecoCustoMedio
            };
        }
    }
}
