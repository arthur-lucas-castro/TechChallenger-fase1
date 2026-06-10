using Application.Servicos.DTOs;
using Application.Servicos.Interfaces;
using Domain.Entidades;
using Domain.Interfaces;

namespace Application.Servicos
{
    public class ItemServicoService : IItemServicoService
    {
        private readonly IItemServicoRepositorio _repositorio;

        public ItemServicoService(IItemServicoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<ItemServicoResponseDTO?> ObterPorIdAsync(int id)
        {
            var item = await _repositorio.GetByIdAsync(id);
            return item is null ? null : MapearParaDTO(item);
        }

        public async Task<IEnumerable<ItemServicoResponseDTO>> ObterTodosAsync()
        {
            var itens = await _repositorio.GetAllAsync();
            return itens.Select(MapearParaDTO);
        }

        public Task<int> CriarAsync(ItemServicoRequestDTO dto)
            => _repositorio.InsertAsync(MapearParaEntidade(dto));

        public async Task<bool> AtualizarAsync(int id, ItemServicoRequestDTO dto)
        {
            var entidade = MapearParaEntidade(dto);
            entidade.Id = id;
            return await _repositorio.UpdateAsync(entidade);
        }

        public Task<bool> ExcluirAsync(int id)
            => _repositorio.DeleteAsync(id);

        private static ItemServicoResponseDTO MapearParaDTO(ItemServico i) => new()
        {
            Id                     = i.Id,
            Nome                   = i.Nome,
            PrecoVenda             = i.PrecoVenda,
            TempoEstimadoEmMinutos = i.TempoEstimadoEmMinutos
        };

        private static ItemServico MapearParaEntidade(ItemServicoRequestDTO dto) => new()
        {
            Nome                   = dto.Nome,
            PrecoVenda             = dto.PrecoVenda,
            TempoEstimadoEmMinutos = dto.TempoEstimadoEmMinutos
        };
    }
}
