using Compartilhado.Domain;
using Compartilhado.Domain.ObjetosDeValor;

namespace Cliente.Domain
{
    public class Veiculo : EntidadeBase<Veiculo>
    {
        public string Modelo { get; set; } = string.Empty;
        public Placa Placa { get; set; } = null!;
        public string Marca { get; set; } = string.Empty;
        public int Ano { get; set; }
    }
}
