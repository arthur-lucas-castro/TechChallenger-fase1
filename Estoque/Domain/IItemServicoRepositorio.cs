namespace Estoque.Domain
{
    public interface IItemServicoRepositorio
    {
        Task<ItemServico?> GetByIdAsync(int id);
        Task<IEnumerable<ItemServico>> GetAllAsync();
        Task<int> InsertAsync(ItemServico itemServico);
        Task<bool> UpdateAsync(ItemServico itemServico);
        Task<bool> DeleteAsync(int id);
    }
}
