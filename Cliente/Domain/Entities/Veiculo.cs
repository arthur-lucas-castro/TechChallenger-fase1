using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;

namespace Cliente.Domain.Entities
{
    public class Veiculo : EntidadeBase<Veiculo>
    {
        public string Modelo { get; set; } = string.Empty;
        public Placa Placa { get; set; } = null!;
        public string Marca { get; set; } = string.Empty;
        public int Ano { get; set; }
    }
}
