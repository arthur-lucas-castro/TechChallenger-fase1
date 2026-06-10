namespace Application.Servicos.DTOs
{
    public class ItemServicoRequestDTO
    {
        public string Nome { get; set; } = string.Empty;
        public decimal PrecoVenda { get; set; }
        public int TempoEstimadoEmMinutos { get; set; }
    }
}
