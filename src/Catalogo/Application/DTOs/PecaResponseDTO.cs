namespace Catalogo.Application.DTOs
{
    public class PecaResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Custo { get; set; }
        public decimal PrecoVenda { get; set; }
    }
}
