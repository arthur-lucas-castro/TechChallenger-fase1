using Compartilhado.Domain.Entities;
using Estoque.Application.DTOs;
using Estoque.Application.Services.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;

namespace Estoque.Application.Services
{
    public class PecaService : IPecaService
    {
        private readonly IPecaRepositorio _repositorio;
        private readonly IDomainEventDispatcher _dispatcher;

        public PecaService(IPecaRepositorio repositorio, IDomainEventDispatcher dispatcher)
        {
            _repositorio = repositorio;
            _dispatcher  = dispatcher;
        }

        public async Task<PecaResponseDTO?> ObterPorIdAsync(int id)
        {
            var peca = await _repositorio.GetByIdAsync(id);
            return peca is null ? null : MapearParaDTO(peca);
        }

        public async Task<IEnumerable<PecaResponseDTO>> ObterTodosAsync()
            => (await _repositorio.GetAllAsync()).Select(MapearParaDTO);

        public Task<int> CriarAsync(PecaRequestDTO dto)
            => _repositorio.InsertAsync(MapearParaEntidade(dto));

        public async Task<bool> AtualizarAsync(int id, PecaRequestDTO dto)
        {
            var peca = MapearParaEntidade(dto);
            peca.Id = id;
            return await _repositorio.UpdateAsync(peca);
        }

        public Task<bool> ExcluirAsync(int id)
            => _repositorio.DeleteAsync(id);

        public async Task<IEnumerable<EstoqueComPecaResponseDTO>> ObterEstoqueTodosAsync()
        {
            var pecas = await _repositorio.GetAllComEstoqueAsync();
            return pecas.Select(p => new EstoqueComPecaResponseDTO
            {
                PecaId           = p.Id,
                NomePeca         = p.Nome,
                DescricaoPeca    = p.Descricao,
                PrecoVendaPeca   = p.PrecoVenda,
                QuantidadeAtual  = p.ProdutoEstoque?.QuantidadeAtual,
                QuantidadeMinima = p.ProdutoEstoque?.QuantidadeMinima,
                PrecoCustoMedio  = p.ProdutoEstoque is not null ? (decimal)p.ProdutoEstoque.PrecoCustoMedio : null
            });
        }

        public async Task<EstoqueResponseDTO> AdicionarEstoqueAsync(EntradaEstoqueRequestDTO dto)
        {
            var peca = await _repositorio.GetByIdComEstoqueAsync(dto.PecaId)
                ?? throw new KeyNotFoundException($"Peça {dto.PecaId} não encontrada.");

            peca.AdicionarEstoque(dto.Quantidade, dto.PrecoCusto);

            if (peca.ProdutoEstoque!.Id == 0)
                peca.ProdutoEstoque.Id = await _repositorio.InsertProdutoEstoqueAsync(peca.ProdutoEstoque);
            else
                await _repositorio.UpdateProdutoEstoqueAsync(peca.ProdutoEstoque);

            return MapearEstoqueParaDTO(peca.ProdutoEstoque);
        }

        public async Task<EstoqueResponseDTO?> DarBaixaAsync(BaixaEstoqueRequestDTO dto)
        {
            var peca = await _repositorio.GetByIdComEstoqueAsync(dto.PecaId);
            if (peca is null) return null;

            peca.DarBaixa(dto.Quantidade);
            var estoque = peca.ProdutoEstoque!;

            await _repositorio.UpdateProdutoEstoqueAsync(estoque);
            await _dispatcher.DispatchAsync(estoque.GetDomainEvents());
            estoque.ClearDomainEvents();

            return MapearEstoqueParaDTO(estoque);
        }

        private static PecaResponseDTO MapearParaDTO(Peca p) => new()
        {
            Id         = p.Id,
            Nome       = p.Nome,
            Descricao  = p.Descricao,
            Custo      = p.Custo,
            PrecoVenda = p.PrecoVenda
        };

        private static Peca MapearParaEntidade(PecaRequestDTO dto) => new()
        {
            Nome       = dto.Nome,
            Descricao  = dto.Descricao,
            Custo      = dto.Custo,
            PrecoVenda = dto.PrecoVenda
        };

        private static EstoqueResponseDTO MapearEstoqueParaDTO(ProdutoEstoque e) => new()
        {
            Id               = e.Id,
            PecaId           = e.PecaId,
            QuantidadeAtual  = e.QuantidadeAtual,
            QuantidadeMinima = e.QuantidadeMinima,
            PrecoCustoMedio  = e.PrecoCustoMedio
        };
    }
}
