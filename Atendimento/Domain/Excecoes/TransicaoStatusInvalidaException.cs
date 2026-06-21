using Compartilhado.Domain.Entities.Exceptions;

namespace Atendimento.Domain.Excecoes;

public class TransicaoStatusInvalidaException : DomainException
{
    public TransicaoStatusInvalidaException(string mensagem) : base(mensagem) { }
}
