using Estoque.Application.DTOs;
using Estoque.Application.Services.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;

namespace Estoque.Application.Services
{
    public class PecaService : IPecaService
    {
        private readonly IPecaRepositorio _repositorio;

        public PecaService(IPecaRepositorio repositorio) => _repositorio = repositorio;

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
    }
}
