using Cliente.Application.DTOs;
using Cliente.Application.Services.Interfaces;
using Cliente.Domain.Entities;
using Cliente.Domain.Interfaces;

namespace Cliente.Application.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IVeiculoRepositorio _repositorio;

        public VeiculoService(IVeiculoRepositorio repositorio) => _repositorio = repositorio;

        public async Task<VeiculoResponseDTO?> ObterPorIdAsync(int id)
        {
            var v = await _repositorio.GetByIdAsync(id);
            return v is null ? null : MapearParaDTO(v);
        }

        public async Task<IEnumerable<VeiculoResponseDTO>> ObterTodosAsync()
            => (await _repositorio.GetAllAsync()).Select(MapearParaDTO);

        public Task<int> CriarAsync(VeiculoRequestDTO dto)
            => _repositorio.InsertAsync(MapearParaEntidade(dto));

        public async Task<bool> AtualizarAsync(int id, VeiculoRequestDTO dto)
        {
            var e = MapearParaEntidade(dto);
            e.Id = id;
            return await _repositorio.UpdateAsync(e);
        }

        public Task<bool> ExcluirAsync(int id) => _repositorio.DeleteAsync(id);

        public async Task<VeiculoResponseDTO?> ObterPorPlacaAsync(string placa)
        {
            var v = await _repositorio.GetByPlacaAsync(placa);
            return v is null ? null : MapearParaDTO(v);
        }

        private static VeiculoResponseDTO MapearParaDTO(Veiculo v) => new()
        {
            Id = v.Id, Modelo = v.Modelo, Placa = v.Placa, Marca = v.Marca, Ano = v.Ano
        };

        private static Veiculo MapearParaEntidade(VeiculoRequestDTO dto) => new()
        {
            Modelo = dto.Modelo, Placa = dto.Placa, Marca = dto.Marca, Ano = dto.Ano
        };
    }
}
