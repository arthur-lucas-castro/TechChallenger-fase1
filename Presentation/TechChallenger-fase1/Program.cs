using Compartilhado.Infrastructure.TypeHandlers;
using Compartilhado.Infrastructure.Base;
using Compartilhado.Infrastructure.Base.Interface;
using Cliente.Domain;
using Cliente.Application;
using Cliente.Application.Interfaces;
using Cliente.Infrastructure;
using Estoque.Domain;
using Estoque.Application;
using Estoque.Application.Interfaces;
using Estoque.Infrastructure;

DapperTypeHandlers.Registrar();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IVeiculoRepositorio, VeiculoRepositorio>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

builder.Services.AddScoped<IItemServicoRepositorio, ItemServicoRepositorio>();
builder.Services.AddScoped<IItemServicoService, ItemServicoService>();

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
