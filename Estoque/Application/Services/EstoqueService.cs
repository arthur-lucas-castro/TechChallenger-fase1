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

        public async Task<EstoqueResponseDTO> AdicionarProdutoAsync(EntradaEstoqueRequestDTO dto)
        {
            var registros = await _repositorio.GetByExpressionAsync(e => e.PecaId == dto.PecaId);
            var estoque = registros.FirstOrDefault();

            if (estoque is null)
            {
                estoque = new EstoqueEntidade
                {
                    PecaId          = dto.PecaId,
                    QuantidadeAtual = dto.Quantidade,
                    QuantidadeMinima = 0,
                    PrecoCustoMedio = dto.PrecoCusto
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

            return MapearParaDTO(estoque);
        }

        private static EstoqueResponseDTO MapearParaDTO(EstoqueEntidade e) => new()
        {
            Id              = e.Id,
            PecaId          = e.PecaId,
            QuantidadeAtual = e.QuantidadeAtual,
            QuantidadeMinima = e.QuantidadeMinima,
            PrecoCustoMedio = e.PrecoCustoMedio
        };
    }
}
