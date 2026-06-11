using Compartilhado.Domain.Entities;
using Compartilhado.Domain.ValueObjects;

namespace OrdemServico.Domain.Entities
{
    public class OrdemServico : EntidadeBase<OrdemServico>
    {
        public int VeiculoId { get; set; }
        public int ClienteId { get; set; }
        public int ResponsavelId { get; set; }
        public StatusOrdemServico Status { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataFinalizacao { get; set; }
    }
}
