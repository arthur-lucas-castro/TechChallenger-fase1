# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Idioma

Responda sempre em português do Brasil (pt-BR).

## Visão Geral do Projeto

**TechChallenger-fase1** é uma API REST ASP.NET Core 8.0 para um desafio técnico. Segue arquitetura em camadas com três projetos principais:
- **TechChallenger-fase1**: Projeto principal da API (Web)
- **Repositorios**: Camada de acesso a dados usando Dapper ORM
- **Entidades**: Definições de entidades/modelos

A API usa PostgreSQL como banco de dados e é conteinerizada com Docker.

## Arquitetura e Camadas

### Estrutura do Projeto

```
TechChallenger-fase1/
├── TechChallenger-fase1.sln          # Solução Visual Studio
├── TechChallenger-fase1/             # Projeto principal da API
│   ├── Program.cs                    # Configuração ASP.NET Core e injeção de dependência
│   ├── Controllers/                  # Endpoints da API
│   │   ├── TesteController.cs        # Controller da entidade principal
│   │   └── WeatherForecastController.cs
│   ├── appsettings.json              # Configuração
│   └── TechChallenger-fase1.csproj
├── Repositorios/                     # Camada de Acesso a Dados
│   ├── Base/
│   │   ├── RepositorioBase.cs        # Classe base CRUD genérica
│   │   ├── ContextoBancoBase.cs      # Gerenciamento de conexão
│   │   ├── SqlConnectionFactory.cs   # Fábrica de conexão com o banco
│   │   └── Interface/
│   │       └── IDbConnectionFactory.cs
│   ├── TesteRepositorio.cs           # Repositório específico para a entidade Teste
│   └── Repositorios.csproj
├── Entidades/                        # Modelos de Entidade
│   ├── Base/
│   │   └── EntidadeBase.cs           # Entidade base genérica
│   ├── Teste.cs                      # Definição da entidade Teste
│   └── Entidades.csproj
├── database/                         # Configuração do banco de dados
│   ├── Dockerfile                    # Container PostgreSQL
│   ├── docker-compose.yml            # Compose do banco
│   └── init/
│       └── 01_schema.sql             # Schema do banco de dados
└── docker-compose.yml                # Compose completo (API + banco)
```

### Padrões Arquiteturais

**Regra de camadas — OBRIGATÓRIO:**
- **Controller → Service → Repository**: o fluxo de dependência deve sempre seguir essa ordem.
- **Controllers nunca acessam repositórios diretamente.** Toda lógica de negócio passa pela camada de serviço.
- **Controllers nunca conhecem entidades de domínio.** A comunicação entre Controller e Application é feita exclusivamente via DTOs.
- **Sempre use interfaces com injeção de dependência.** Nunca injete implementações concretas de serviços ou repositórios diretamente — injete a interface (`IClienteService`, `IClienteRepositorio`, etc.).

**Fluxo obrigatório por camada:**
```
Presentation (Controller)
    ↕ DTOs (ClienteRequestDTO / ClienteResponseDTO)
Application (IServiço → Serviço)
    ↕ Entidades de domínio
Infrastructure (IRepositorio → Repositorio)
    ↕ Entidades de domínio
Domain (Entidade)
```

**Localização dos artefatos:**
- DTOs ficam em `Application/Servicos/DTOs/` — ex.: `ClienteRequestDTO`, `ClienteResponseDTO`
- Interfaces de **repositório** ficam em `Domain/Interfaces/` — ex.: `IClienteRepositorio`
- Interfaces de **serviço** ficam em `Application/Servicos/Interfaces/` — ex.: `IClienteService`
- O mapeamento entre DTO e Entidade é responsabilidade do **Serviço**, nunca do Controller
- Registros no DI sempre na forma: `AddScoped<IClienteService, ClienteService>()`

**Padrão Repository Genérico**: `RepositorioBase<TEntidade>` fornece operações CRUD (GetById, GetAll, Insert, Update, Delete) para qualquer entidade usando reflexão com Dapper. Os nomes de tabela são derivados como `typeof(TEntidade).Name + "s"` (ex.: `Teste` → `Testes`), que o PostgreSQL resolve sem diferenciação de maiúsculas para corresponder aos nomes em minúsculo no schema SQL, como `testes`.

**Injeção de Dependência (DI)**: O `Program.cs` registra:
- `IDbConnectionFactory` → `SqlConnectionFactory` (pool de conexões)
- `IClienteRepositorio` → `ClienteRepositorio` (padrão para repositórios)
- `IClienteService` → `ClienteService` (padrão para serviços)

**Acesso a Dados**: Usa Dapper como micro-ORM com queries SQL construídas via reflexão nas propriedades das entidades.

## Comandos de Build e Execução

### Pré-requisitos
- .NET 8.0 SDK
- Docker & Docker Compose (para o banco de dados)
- PostgreSQL 16 (ou usar Docker)

### Build
```bash
dotnet build TechChallenger-fase1.sln
```

### Executar a API (requer banco de dados)
```bash
dotnet run --project TechChallenger-fase1
```
A API inicia em `https://localhost:5001` (ou porta configurada), Swagger UI em `/swagger`

### Banco de Dados com Docker
```bash
# Iniciar container PostgreSQL com schema
docker-compose up database

# Ou stack completa (API + banco)
docker-compose up
```

### Modo Watch (rebuild automático ao alterar arquivos)
```bash
dotnet watch run --project TechChallenger-fase1
```

### Publicar
```bash
dotnet publish TechChallenger-fase1.sln
```
Saída em `./publish/` para deploy via Docker.

## Configuração

**appsettings.json** (TechChallenger-fase1):
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=techchallenger;Username=postgres;Password=postgres"
  },
  "Logging": { ... }
}
```

Ao rodar no Docker via `docker-compose.yml`, a connection string é sobrescrita por variável de ambiente:
```
ConnectionStrings__Default=Host=postgres;Port=5432;Database=techchallenger;Username=postgres;Password=postgres
```

## Banco de Dados

**Schema** (`database/init/01_schema.sql`):
```sql
CREATE TABLE testes (
    id        SERIAL PRIMARY KEY,
    descricao VARCHAR(50) NOT NULL
);
```

PostgreSQL roda na porta 5432 no Docker (nome do container: `techchallenger-db`).

Para adicionar novas tabelas ou modificar o schema:
1. Criar/editar arquivos SQL em `database/init/`
2. Recriar o container: `docker-compose down && docker-compose up database`

## Adicionando Novas Entidades e Endpoints

### 1. Criar a Entidade (`Domain/Entidades/`)
```csharp
public class SuaEntidade : EntidadeBase<SuaEntidade>
{
    public string Nome { get; set; } = string.Empty;
}
```

### 2. Criar a Tabela (`database/init/`)
```sql
CREATE TABLE SuaEntidade (
    Id    SERIAL PRIMARY KEY,
    Nome  VARCHAR(100) NOT NULL
);
```

### 3. Criar a interface e o repositório (`Domain/Interfaces/` e `Infrastructure/Repositorios/`)
```csharp
// Domain/Interfaces/ISuaEntidadeRepositorio.cs
public interface ISuaEntidadeRepositorio
{
    Task<SuaEntidade?> GetByIdAsync(int id);
    Task<IEnumerable<SuaEntidade>> GetAllAsync();
    Task<int> InsertAsync(SuaEntidade entidade);
    Task<bool> UpdateAsync(SuaEntidade entidade);
    Task<bool> DeleteAsync(int id);
}

// Infrastructure/Repositorios/SuaEntidadeRepositorio.cs
public class SuaEntidadeRepositorio : RepositorioBase<SuaEntidade>, ISuaEntidadeRepositorio
{
    public SuaEntidadeRepositorio(IDbConnectionFactory connectionFactory)
        : base(connectionFactory) { }
}
```

### 4. Criar a interface e o serviço (`Domain/Interfaces/` e `Application/Servicos/`)
```csharp
// Domain/Interfaces/ISuaEntidadeService.cs
public interface ISuaEntidadeService
{
    Task<SuaEntidade?> ObterPorIdAsync(int id);
    Task<IEnumerable<SuaEntidade>> ObterTodosAsync();
    Task<int> CriarAsync(SuaEntidade entidade);
    Task<bool> AtualizarAsync(int id, SuaEntidade entidade);
    Task<bool> ExcluirAsync(int id);
}

// Application/Servicos/SuaEntidadeService.cs
public class SuaEntidadeService : ISuaEntidadeService
{
    private readonly ISuaEntidadeRepositorio _repositorio;

    public SuaEntidadeService(ISuaEntidadeRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<SuaEntidade?> ObterPorIdAsync(int id) => _repositorio.GetByIdAsync(id);
    public Task<IEnumerable<SuaEntidade>> ObterTodosAsync() => _repositorio.GetAllAsync();
    public Task<int> CriarAsync(SuaEntidade entidade) => _repositorio.InsertAsync(entidade);
    public async Task<bool> AtualizarAsync(int id, SuaEntidade entidade)
    {
        entidade.Id = id;
        return await _repositorio.UpdateAsync(entidade);
    }
    public Task<bool> ExcluirAsync(int id) => _repositorio.DeleteAsync(id);
}
```

### 5. Registrar na DI (`Program.cs`)
```csharp
builder.Services.AddScoped<ISuaEntidadeRepositorio, SuaEntidadeRepositorio>();
builder.Services.AddScoped<ISuaEntidadeService, SuaEntidadeService>();
```

### 6. Criar o Controller (`Presentation/TechChallenger-fase1/Controllers/`)
```csharp
[ApiController]
[Route("[controller]")]
public class SuaEntidadeController : ControllerBase
{
    private readonly ISuaEntidadeService _service;

    public SuaEntidadeController(ISuaEntidadeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.ObterTodosAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entidade = await _service.ObterPorIdAsync(id);
        if (entidade is null) return NotFound();
        return Ok(entidade);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SuaEntidade entidade)
    {
        var id = await _service.CriarAsync(entidade);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SuaEntidade entidade)
    {
        if (!await _service.AtualizarAsync(id, entidade)) return NotFound();
        return Ok(entidade);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _service.ExcluirAsync(id)) return NotFound();
        return NoContent();
    }
}
```

## Testes e Desenvolvimento

- **VSCode Tasks**: Configuradas em `.vscode/tasks.json` para build, publish e watch
- **Depurador**: Configurado em `.vscode/launch.json` para debug .NET Core
- Sem testes unitários no momento; testes devem ficar em um novo projeto `Tests/`

## Dependências

**Projeto Principal** (TechChallenger-fase1):
- Swashbuckle.AspNetCore 6.6.2 (Swagger/OpenAPI)

**Projeto Repositorios**:
- Dapper 2.1.72 (Micro-ORM)
- Dommel 3.5.1 (extensões Dapper)
- Npgsql 8.0.5 (driver PostgreSQL)
- Microsoft.Extensions.Configuration.Abstractions 10.0.7 (configuração)

**Projeto Entidades**: Sem dependências externas
