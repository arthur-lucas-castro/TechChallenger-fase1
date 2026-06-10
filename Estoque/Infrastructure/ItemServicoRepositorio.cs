using Estoque.Domain;
using Compartilhado.Infrastructure.Base;
using Compartilhado.Infrastructure.Base.Interface;

namespace Estoque.Infrastructure
{
    public class ItemServicoRepositorio : RepositorioBase<ItemServico>, IItemServicoRepositorio
    {
        public ItemServicoRepositorio(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }
}
