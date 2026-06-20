using Microsoft.EntityFrameworkCore;
using Cliente.Domain.Entities;
using Cliente.Domain.ValueObjects;
using Estoque.Domain.Entities;
using Estoque.Domain.ValueObjects;
using Atendimento.Domain.Entities;
using Compartilhado.Domain.ValueObjects;
using AtendimentoOrdemServico = Atendimento.Domain.Entities.OrdemServico;

namespace Compartilhado.Infrastructure.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente.Domain.Entities.Cliente> Cliente { get; set; }
        public DbSet<Veiculo> Veiculo { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<Peca> Peca { get; set; }
        public DbSet<AtendimentoOrdemServico> OrdemServico { get; set; }
        public DbSet<ServicoSolicitado> ServicoSolicitado { get; set; }
        public DbSet<PecaSolicitada> PecaSolicitada { get; set; }
        public DbSet<ProdutoEstoque> Estoque { get; set; }
        public DbSet<Orcamento> Orcamento { get; set; }
        public DbSet<ServicoExecucao> ServicoExecucao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<StatusOrdemServico>("status_ordem_servico");
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

            modelBuilder.Entity<Servico>(b =>
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

            modelBuilder.Entity<AtendimentoOrdemServico>(b =>
            {
                b.HasMany(x => x.ServicosSolicitados)
                    .WithOne()
                    .HasForeignKey(s => s.OrdemServicoId);
                b.HasMany(x => x.PecasSolicitadas)
                    .WithOne()
                    .HasForeignKey(p => p.OrdemServicoId);
                b.HasOne(x => x.Orcamento)
                    .WithOne()
                    .HasForeignKey<Orcamento>(o => o.OrdemServicoId);
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

            modelBuilder.Entity<ServicoSolicitado>(b =>
            {
                b.Property(x => x.PrecoVenda)
                    .HasConversion(v => v.Valor, v => new Dinheiro(v))
                    .HasColumnType("numeric(10,2)");
                b.HasOne(x => x.ServicoExecucao)
                    .WithOne()
                    .HasForeignKey<ServicoExecucao>(e => e.ServicoSolicitadoId);
            });

            modelBuilder.Entity<ServicoExecucao>(b =>
            {
                b.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<PecaSolicitada>(b =>
            {
                b.Property(x => x.PrecoVenda)
                    .HasConversion(v => v.Valor, v => new Dinheiro(v))
                    .HasColumnType("numeric(10,2)");
            });


            modelBuilder.Entity<ProdutoEstoque>(b =>
            {
                b.ToTable("produtoestoque");
                b.Property(x => x.PrecoCustoMedio)
                    .HasConversion(v => v.Valor, v => new Dinheiro(v))
                    .HasColumnType("numeric(10,2)");
            });
        }
    }
}
