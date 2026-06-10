using Estoque.Application.DTOs;
using Estoque.Application.Interfaces;
using Estoque.Domain;

namespace Estoque.Application
{
    public class ItemServicoService : IItemServicoService
    {
        private readonly IItemServicoRepositorio _repositorio;

        public ItemServicoService(IItemServicoRepositorio repositorio) => _repositorio = repositorio;

        public async Task<ItemServicoResponseDTO?> ObterPorIdAsync(int id)
        {
            var i = await _repositorio.GetByIdAsync(id);
            return i is null ? null : MapearParaDTO(i);
        }

        public async Task<IEnumerable<ItemServicoResponseDTO>> ObterTodosAsync()
            => (await _repositorio.GetAllAsync()).Select(MapearParaDTO);

        public Task<int> CriarAsync(ItemServicoRequestDTO dto)
            => _repositorio.InsertAsync(MapearParaEntidade(dto));

        public async Task<bool> AtualizarAsync(int id, ItemServicoRequestDTO dto)
        {
            var e = MapearParaEntidade(dto);
            e.Id = id;
            return await _repositorio.UpdateAsync(e);
        }

        public Task<bool> ExcluirAsync(int id) => _repositorio.DeleteAsync(id);

        private static ItemServicoResponseDTO MapearParaDTO(ItemServico i) => new()
        {
            Id = i.Id, Nome = i.Nome, PrecoVenda = i.PrecoVenda,
            TempoEstimadoEmMinutos = i.TempoEstimadoEmMinutos
        };

        private static ItemServico MapearParaEntidade(ItemServicoRequestDTO dto) => new()
        {
            Nome = dto.Nome, PrecoVenda = dto.PrecoVenda,
            TempoEstimadoEmMinutos = dto.TempoEstimadoEmMinutos
        };
    }
}
