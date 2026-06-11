using Cliente.Domain.Entities;
using Cliente.Domain.Interfaces;
using Compartilhado.Infrastructure.Repositories;
using Compartilhado.Infrastructure.Repositories.Interface;

namespace Cliente.Infrastructure.Repositories
{
    public class VeiculoRepositorio : RepositorioBase<Veiculo>, IVeiculoRepositorio
    {
        public VeiculoRepositorio(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }
}
