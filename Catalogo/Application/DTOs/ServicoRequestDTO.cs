namespace Catalogo.Application.DTOs
{
    public class ServicoRequestDTO
    {
        public string Nome { get; set; } = string.Empty;
        public decimal PrecoVenda { get; set; }
        public int TempoEstimadoEmMinutos { get; set; }
    }
}
