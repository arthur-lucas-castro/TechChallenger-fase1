using Application.Servicos.DTOs;
using Application.Servicos.Interfaces;
using Domain.Entidades;
using Domain.Interfaces;

namespace Application.Servicos
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IVeiculoRepositorio _repositorio;

        public VeiculoService(IVeiculoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<VeiculoResponseDTO?> ObterPorIdAsync(int id)
        {
            var veiculo = await _repositorio.GetByIdAsync(id);
            return veiculo is null ? null : MapearParaDTO(veiculo);
        }

        public async Task<IEnumerable<VeiculoResponseDTO>> ObterTodosAsync()
        {
            var veiculos = await _repositorio.GetAllAsync();
            return veiculos.Select(MapearParaDTO);
        }

        public Task<int> CriarAsync(VeiculoRequestDTO dto)
            => _repositorio.InsertAsync(MapearParaEntidade(dto));

        public async Task<bool> AtualizarAsync(int id, VeiculoRequestDTO dto)
        {
            var entidade = MapearParaEntidade(dto);
            entidade.Id = id;
            return await _repositorio.UpdateAsync(entidade);
        }

        public Task<bool> ExcluirAsync(int id)
            => _repositorio.DeleteAsync(id);

        private static VeiculoResponseDTO MapearParaDTO(Veiculo v) => new()
        {
            Id     = v.Id,
            Modelo = v.Modelo,
            Placa  = v.Placa,
            Marca  = v.Marca,
            Ano    = v.Ano
        };

        private static Veiculo MapearParaEntidade(VeiculoRequestDTO dto) => new()
        {
            Modelo = dto.Modelo,
            Placa  = dto.Placa,
            Marca  = dto.Marca,
            Ano    = dto.Ano
        };
    }
}
