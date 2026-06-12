using Microsoft.EntityFrameworkCore;
using Cliente.Domain.Entities;
using Estoque.Domain.Entities;
using OrdemServico.Domain.Entities;
using Compartilhado.Domain.ValueObjects;
using OrdemServicoEntidade = OrdemServico.Domain.Entities.OrdemServico;

namespace Compartilhado.Infrastructure.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente.Domain.Entities.Cliente> Clientes { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<ClienteVeiculo> ClienteVeiculos { get; set; }
        public DbSet<ItemServico> ItemServicos { get; set; }
        public DbSet<Peca> Pecas { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<OrdemServicoEntidade> OrdemServicos { get; set; }
        public DbSet<OrdemServicoItem> OrdemServicoItems { get; set; }
        public DbSet<OrdemServicoInsumo> OrdemServicoInsumos { get; set; }
        public DbSet<Orcamento> Orcamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClienteVeiculo>()
                .HasKey(x => new { x.ClienteId, x.VeiculoId });

            modelBuilder.Entity<Cliente.Domain.Entities.Cliente>(b =>
            {
                b.Property(x => x.Telefone)
                    .HasConversion(v => v.Valor, v => new Telefone(v));
                b.Property(x => x.Email)
                    .HasConversion(v => v.Valor, v => new Email(v));
                b.Property(x => x.NumeroDocumento)
                    .HasConversion(v => v.Numero, v => new Documento(v));
                b.Property(x => x.TipoPessoa)
                    .HasConversion<string>()
                    .HasMaxLength(1);
            });

            modelBuilder.Entity<Veiculo>(b =>
            {
                b.Property(x => x.Placa)
                    .HasConversion(v => v.Valor, v => new Placa(v))
                    .HasMaxLength(7);
            });

            modelBuilder.Entity<ItemServico>(b =>
            {
                b.Property(x => x.PrecoVenda)
                    .HasConversion(v => v.Valor, v => new Dinheiro(v))
                    .HasColumnType("numeric(10,2)");
            });

            modelBuilder.Entity<Peca>(b =>
            {
                b.Property(x => x.Custo)
                    .HasConversion(v => v.Valor, v => new Dinheiro(v))
                    .HasColumnType("numeric(10,2)");
                b.Property(x => x.PrecoVenda)
                    .HasConversion(v => v.Valor, v => new Dinheiro(v))
                    .HasColumnType("numeric(10,2)");
            });

            modelBuilder.Entity<OrdemServicoEntidade>(b =>
            {
                b.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<OrdemServicoItem>(b =>
            {
                b.Property(x => x.Preco)
                    .HasConversion(v => v.Valor, v => new Dinheiro(v))
                    .HasColumnType("numeric(10,2)");
            });

            modelBuilder.Entity<Orcamento>(b =>
            {
                b.Property(x => x.PrecoTotal)
                    .HasConversion(v => v.Valor, v => new Dinheiro(v))
                    .HasColumnType("numeric(10,2)");
                b.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });
        }
    }
}
