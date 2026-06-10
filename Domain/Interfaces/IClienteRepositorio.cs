using Domain.Entidades;

namespace Domain.Interfaces
{
    public interface IClienteRepositorio
    {
        Task<Cliente?> GetByIdAsync(int id);
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<int> InsertAsync(Cliente cliente);
        Task<bool> UpdateAsync(Cliente cliente);
        Task<bool> DeleteAsync(int id);
    }
}
