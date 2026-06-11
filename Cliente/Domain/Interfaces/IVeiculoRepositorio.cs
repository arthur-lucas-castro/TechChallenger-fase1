using Cliente.Domain.Entities;

namespace Cliente.Domain.Interfaces
{
    public interface IVeiculoRepositorio
    {
        Task<Veiculo?> GetByIdAsync(int id);
        Task<IEnumerable<Veiculo>> GetAllAsync();
        Task<int> InsertAsync(Veiculo veiculo);
        Task<bool> UpdateAsync(Veiculo veiculo);
        Task<bool> DeleteAsync(int id);
    }
}
