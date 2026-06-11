using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;
using Compartilhado.Infrastructure.Repositories;
using Compartilhado.Infrastructure.Repositories.Interface;

namespace Estoque.Infrastructure.Repositories
{
    public class ItemServicoRepositorio : RepositorioBase<ItemServico>, IItemServicoRepositorio
    {
        public ItemServicoRepositorio(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }
}
