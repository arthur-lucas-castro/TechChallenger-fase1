using Compartilhado.Infrastructure.Repositories;
using Compartilhado.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Cliente.Domain.Interfaces;
using Cliente.Application.Services;
using Cliente.Application.Services.Interfaces;
using Cliente.Infrastructure.Repositories;
using Estoque.Domain.Interfaces;
using Estoque.Domain.Entities.Events;
using Estoque.Application.Services;
using Estoque.Application.Services.Interfaces;
using Estoque.Application.Services.Events;
using Estoque.Infrastructure.Repositories;
using Atendimento.Domain.Interfaces;
using Atendimento.Application.Services;
using Atendimento.Application.Services.Interfaces;
using Atendimento.Application.Services.Events;
using Atendimento.Domain.Entities.Events;
using Atendimento.Infrastructure.Repositories;
using Npgsql;
using Compartilhado.Domain.ValueObjects;


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Default")!);
dataSourceBuilder.MapEnum<StatusOrdemServico>("status_ordem_servico");
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dataSource)
           .UseLowerCaseNamingConvention());

builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IVeiculoRepositorio, VeiculoRepositorio>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

builder.Services.AddScoped<IServicoRepositorio, ServicoRepositorio>();
builder.Services.AddScoped<IServicoService, ServicoService>();

builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<IDomainEventHandler<EstoqueBaixaRealizadaEvent>, EstoqueBaixaRealizadaHandler>();

builder.Services.AddScoped<IPecaRepositorio, PecaRepositorio>();
builder.Services.AddScoped<IPecaService, PecaService>();

builder.Services.AddScoped<IOrdemServicoRepositorio, OrdemServicoRepositorio>();
builder.Services.AddScoped<IOrdemServicoService, OrdemServicoService>();
builder.Services.AddScoped<IDomainEventHandler<OrdemServicoDiagnosticoFinalizadoEvent>, OrdemServicoDiagnosticoFinalizadoHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
