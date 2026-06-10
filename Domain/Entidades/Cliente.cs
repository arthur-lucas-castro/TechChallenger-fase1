using Domain.Entidades.Base;
using Domain.ObjetosDeValor;

namespace Domain.Entidades
{
    public class Cliente : EntidadeBase<Cliente>
    {
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public Telefone Telefone { get; set; } = null!;
        public Email Email { get; set; } = null!;
        public Documento NumeroDocumento { get; set; } = null!;
        public TipoPessoa TipoPessoa { get; set; }
    }
}

