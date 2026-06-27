namespace Operacao.Application.DTOs
{
    public class OrdemServicoRequestDTO
    {
        public int VeiculoId { get; set; }
        public int ClienteId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ServicoSolicitadoRequestDTO> Servicos { get; set; } = [];
        public List<PecaSolicitadaRequestDTO> Pecas { get; set; } = [];
    }
}
