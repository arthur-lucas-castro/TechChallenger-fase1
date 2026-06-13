using Compartilhado.Infrastructure.Repositories.TypeHandlers;
using Compartilhado.Infrastructure.Repositories;
using Compartilhado.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Cliente.Domain.Interfaces;
using Cliente.Application.Services;
using Cliente.Application.Services.Interfaces;
using Cliente.Infrastructure.Repositories;
using Estoque.Domain.Interfaces;
using Estoque.Application.Services;
using Estoque.Application.Services.Interfaces;
using Estoque.Infrastructure.Repositories;

DapperTypeHandlers.Registrar();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
           .UseLowerCaseNamingConvention());

builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IVeiculoRepositorio, VeiculoRepositorio>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

builder.Services.AddScoped<IItemServicoRepositorio, ItemServicoRepositorio>();
builder.Services.AddScoped<IItemServicoService, ItemServicoService>();

builder.Services.AddScoped<IEstoqueRepositorio, EstoqueRepositorio>();
builder.Services.AddScoped<IEstoqueService, EstoqueService>();

builder.Services.AddScoped<IPecaRepositorio, PecaRepositorio>();
builder.Services.AddScoped<IPecaService, PecaService>();

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
