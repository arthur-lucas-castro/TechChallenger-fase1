using Estoque.Application.DTOs;
using Estoque.Application.Services.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;

namespace Estoque.Application.Services
{
    public class ServicoService : IServicoService
    {
        private readonly IServicoRepositorio _repositorio;

        public ServicoService(IServicoRepositorio repositorio) => _repositorio = repositorio;

        public async Task<ServicoResponseDTO?> ObterPorIdAsync(int id)
        {
            var s = await _repositorio.GetByIdAsync(id);
            return s is null ? null : MapearParaDTO(s);
        }

        public async Task<IEnumerable<ServicoResponseDTO>> ObterTodosAsync()
            => (await _repositorio.GetAllAsync()).Select(MapearParaDTO);

        public Task<int> CriarAsync(ServicoRequestDTO dto)
            => _repositorio.InsertAsync(MapearParaEntidade(dto));

        public async Task<bool> AtualizarAsync(int id, ServicoRequestDTO dto)
        {
            var e = MapearParaEntidade(dto);
            e.Id = id;
            return await _repositorio.UpdateAsync(e);
        }

        public Task<bool> ExcluirAsync(int id) => _repositorio.DeleteAsync(id);

        private static ServicoResponseDTO MapearParaDTO(Servico s) => new()
        {
            Id = s.Id, Nome = s.Nome, PrecoVenda = s.PrecoVenda,
            TempoEstimadoEmMinutos = s.TempoEstimadoEmMinutos
        };

        private static Servico MapearParaEntidade(ServicoRequestDTO dto) => new()
        {
            Nome = dto.Nome, PrecoVenda = dto.PrecoVenda,
            TempoEstimadoEmMinutos = dto.TempoEstimadoEmMinutos
        };
    }
}
