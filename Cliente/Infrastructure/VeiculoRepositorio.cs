using Cliente.Domain;
using Compartilhado.Infrastructure.Base;
using Compartilhado.Infrastructure.Base.Interface;

namespace Cliente.Infrastructure
{
    public class VeiculoRepositorio : RepositorioBase<Veiculo>, IVeiculoRepositorio
    {
        public VeiculoRepositorio(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }
}
