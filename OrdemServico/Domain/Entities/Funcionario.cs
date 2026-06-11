using Compartilhado.Domain.Entities;

namespace OrdemServico.Domain.Entities
{
    public class Funcionario : EntidadeBase<Funcionario>
    {
        public string Nome { get; set; } = string.Empty;
    }
}
