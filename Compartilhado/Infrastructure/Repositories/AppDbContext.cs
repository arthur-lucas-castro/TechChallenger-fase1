using Microsoft.EntityFrameworkCore;
using Cliente.Domain.Entities;
using Estoque.Domain.Entities;
using OrdemServico.Domain.Entities;
using Compartilhado.Domain.ValueObjects;
using OrdemServicoEntidade = OrdemServico.Domain.Entities.OrdemServico;
using EstoqueEntidade = Estoque.Domain.Entities.Estoque;

namespace Compartilhado.Infrastructure.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente.Domain.Entities.Cliente> Cliente { get; set; }
        public DbSet<Veiculo> Veiculo { get; set; }
        public DbSet<ClienteVeiculo> ClienteVeiculo { get; set; }
        public DbSet<ItemServico> ItemServico { get; set; }
        public DbSet<Peca> Peca { get; set; }
        public DbSet<Funcionario> Funcionario { get; set; }
        public DbSet<OrdemServicoEntidade> OrdemServico { get; set; }
        public DbSet<OrdemServicoItem> OrdemServicoItem { get; set; }
        public DbSet<OrdemServicoInsumo> OrdemServicoInsumo { get; set; }
        public DbSet<Orcamento> Orcamento { get; set; }
        public DbSet<EstoqueEntidade> Estoque { get; set; }

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

            modelBuilder.Entity<EstoqueEntidade>(b =>
            {
                b.Property(x => x.PrecoCustoMedio)
                    .HasConversion(v => v.Valor, v => new Dinheiro(v))
                    .HasColumnType("numeric(10,2)");
                b.HasOne(x => x.Peca)
                    .WithMany()
                    .HasForeignKey(x => x.PecaId);
            });
        }
    }
}
