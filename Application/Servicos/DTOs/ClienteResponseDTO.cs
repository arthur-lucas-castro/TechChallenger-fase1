using Domain.ObjetosDeValor;

namespace Application.Servicos.DTOs
{
    public class ClienteResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public TipoPessoa TipoPessoa { get; set; }
    }
}
