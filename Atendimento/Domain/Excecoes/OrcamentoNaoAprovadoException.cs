using Compartilhado.Domain.Entities.Exceptions;

namespace Atendimento.Domain.Excecoes;

public class OrcamentoNaoAprovadoException : DomainException
{
    public OrcamentoNaoAprovadoException(string mensagem) : base(mensagem) { }
}
