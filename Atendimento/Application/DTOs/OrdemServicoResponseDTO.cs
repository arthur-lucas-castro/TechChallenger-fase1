namespace Atendimento.Application.DTOs
{
    public class OrdemServicoResponseDTO
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public int ClienteId { get; set; }
        public int ResponsavelId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public DateTime? DataFinalizacao { get; set; }
    }
}
