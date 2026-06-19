using Compartilhado.Domain.Entities;

namespace Atendimento.Domain.Entities
{
    public class Veiculo : EntidadeBase<Veiculo>
    {
        public string Modelo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Placa { get; set; } = string.Empty;
    }
}
