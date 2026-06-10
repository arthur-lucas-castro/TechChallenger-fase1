using Domain.Entidades;
using Domain.Interfaces;
using Infrastructure.Repositorios.Base;
using Infrastructure.Repositorios.Base.Interface;

namespace Infrastructure.Repositorios
{
    public class VeiculoRepositorio : RepositorioBase<Veiculo>, IVeiculoRepositorio
    {
        public VeiculoRepositorio(IDbConnectionFactory connectionFactory)
            : base(connectionFactory) { }
    }
}
