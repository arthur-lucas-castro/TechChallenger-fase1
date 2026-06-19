using Atendimento.Domain.Entities;
using Atendimento.Domain.Interfaces;
using Compartilhado.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Atendimento.Infrastructure.Repositories
{
    public class OrdemServicoRepositorio : BaseRepository<OrdemServico>, IOrdemServicoRepositorio
    {
        public OrdemServicoRepositorio(AppDbContext context) : base(context) { }

        public async Task<OrdemServico?> GetByIdComServicosAsync(int id)
            => await _context.Set<OrdemServico>()
                .Include(o => o.ServicosSolicitados)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<OrdemServico?> GetByIdComPecasAsync(int id)
            => await _context.Set<OrdemServico>()
                .Include(o => o.PecasSolicitadas)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<OrdemServico?> GetByIdComItensAsync(int id)
            => await _context.Set<OrdemServico>()
                .Include(o => o.ServicosSolicitados)
                .Include(o => o.PecasSolicitadas)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<OrdemServico?> GetByIdComServicosEExecucaoAsync(int id)
            => await _context.Set<OrdemServico>()
                .Include(o => o.ServicosSolicitados)
                    .ThenInclude(s => s.ServicoExecucao)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<IEnumerable<OrdemServico>> GetAllComClienteEVeiculoAsync()
        {
            var ordensServico = await _dbSet.ToListAsync();

            var clienteIds = ordensServico.Select(os => os.ClienteId).Distinct().ToList();
            var clientes = await _context.Cliente
                .Where(c => clienteIds.Contains(c.Id))
                .Select(c => new { c.Id, c.Nome, c.Sobrenome })
                .ToListAsync();
            var clienteMap = clientes.ToDictionary(c => c.Id);

            var veiculoIds = ordensServico.Select(os => os.VeiculoId).Distinct().ToList();
            var veiculos = await _context.Veiculo
                .Where(v => veiculoIds.Contains(v.Id))
                .ToListAsync();
            var veiculoMap = veiculos.ToDictionary(v => v.Id);

            foreach (var os in ordensServico)
            {
                if (clienteMap.TryGetValue(os.ClienteId, out var c))
                    os.Cliente = new Atendimento.Domain.Entities.Cliente { Nome = c.Nome, Sobrenome = c.Sobrenome };

                if (veiculoMap.TryGetValue(os.VeiculoId, out var v))
                    os.Veiculo = new Atendimento.Domain.Entities.Veiculo
                    {
                        Modelo = v.Modelo,
                        Marca = v.Marca,
                        Ano = v.Ano,
                        Placa = v.Placa.Valor
                    };
            }

            return ordensServico;
        }

        public async Task<bool> CommitAsync()
            => await _context.SaveChangesAsync() > 0;
    }
}
