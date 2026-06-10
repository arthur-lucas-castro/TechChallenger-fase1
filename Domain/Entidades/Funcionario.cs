using Domain.Entidades.Base;

namespace Domain.Entidades
{
    public class Funcionario : EntidadeBase<Funcionario>
    {
        public string Nome { get; set; } = string.Empty;
    }
}

