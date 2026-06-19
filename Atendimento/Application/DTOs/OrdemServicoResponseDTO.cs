namespace Atendimento.Application.DTOs
{
    public class OrdemServicoResponseDTO
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public string? ModeloVeiculo { get; set; }
        public string? MarcaVeiculo { get; set; }
        public int? AnoVeiculo { get; set; }
        public string? PlacaVeiculo { get; set; }
        public int ClienteId { get; set; }
        public string? NomeCliente { get; set; }
        public string? SobrenomeCliente { get; set; }
        public int ResponsavelId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public DateTime? DataFinalizacao { get; set; }
    }
}
