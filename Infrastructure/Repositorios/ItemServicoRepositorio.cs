using Domain.Entidades;
using Domain.Interfaces;
using Infrastructure.Repositorios.Base;
using Infrastructure.Repositorios.Base.Interface;

namespace Infrastructure.Repositorios
{
    public class ItemServicoRepositorio : RepositorioBase<ItemServico>, IItemServicoRepositorio
    {
        public ItemServicoRepositorio(IDbConnectionFactory connectionFactory)
            : base(connectionFactory) { }
    }
}
