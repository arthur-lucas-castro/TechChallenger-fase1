namespace Estoque.Application.DTOs
{
    public class ItemServicoResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal PrecoVenda { get; set; }
        public int TempoEstimadoEmMinutos { get; set; }
    }
}
