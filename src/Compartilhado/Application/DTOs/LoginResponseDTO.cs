namespace Compartilhado.Application.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiracao { get; set; }
        public string Tipo { get; set; } = string.Empty;
    }
}
