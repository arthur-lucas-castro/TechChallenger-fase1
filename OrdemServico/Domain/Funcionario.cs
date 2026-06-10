using Compartilhado.Domain;

namespace OrdemServico.Domain
{
    public class Funcionario : EntidadeBase<Funcionario>
    {
        public string Nome { get; set; } = string.Empty;
    }
}
