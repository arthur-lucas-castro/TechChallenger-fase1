namespace Atendimento.Application.DTOs
{
    public class TempoExecucaoServicoResponseDTO
    {
        public int ServicoId { get; set; }
        public string NomeServico { get; set; } = string.Empty;
        public double TempoMedioEmMinutos { get; set; }
        public double PiorTempoEmMinutos { get; set; }
    }
}
