using Domain.Entidades;
using Domain.Interfaces;
using Infrastructure.Repositorios.Base;
using Infrastructure.Repositorios.Base.Interface;

namespace Infrastructure.Repositorios
{
    public class ClienteRepositorio : RepositorioBase<Cliente>, IClienteRepositorio
    {
        public ClienteRepositorio(IDbConnectionFactory connectionFactory)
            : base(connectionFactory) { }
    }
}
