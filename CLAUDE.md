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

**Padrão Repository Genérico**: `RepositorioBase<TEntidade>` fornece operações CRUD (GetById, GetAll, Insert, Update, Delete) para qualquer entidade usando reflexão com Dapper. Os nomes de tabela são derivados como `typeof(TEntidade).Name + "s"` (ex.: `Teste` → `Testes`), que o PostgreSQL resolve sem diferenciação de maiúsculas para corresponder aos nomes em minúsculo no schema SQL, como `testes`.

**Injeção de Dependência (DI)**: O `Program.cs` registra:
- `IDbConnectionFactory` → `SqlConnectionFactory` (pool de conexões)
- `TesteRepositorio` (repositórios específicos registrados como scoped)

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

### 1. Criar a Entidade (projeto Entidades)
```csharp
// Entidades/SuaEntidade.cs
public class SuaEntidade : EntidadeBase<SuaEntidade>
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}
```

### 2. Criar a Tabela
```sql
-- database/init/02_suaentidade.sql
CREATE TABLE suaentidades (
    id    SERIAL PRIMARY KEY,
    nome  VARCHAR(100) NOT NULL
);
```

### 3. Criar o Repositório (projeto Repositorios)
```csharp
// Repositorios/SuaEntidadeRepositorio.cs
public class SuaEntidadeRepositorio : RepositorioBase<SuaEntidade>
{
    public SuaEntidadeRepositorio(IDbConnectionFactory connectionFactory)
        : base(connectionFactory) { }
}
```

### 4. Registrar na DI (Program.cs)
```csharp
builder.Services.AddScoped<SuaEntidadeRepositorio>();
```

### 5. Criar o Controller (TechChallenger-fase1/Controllers)
```csharp
[ApiController]
[Route("[controller]")]
public class SuaEntidadeController : ControllerBase
{
    private readonly SuaEntidadeRepositorio _repo;
    
    public SuaEntidadeController(SuaEntidadeRepositorio repo) => _repo = repo;
    
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SuaEntidade entidade)
    {
        var id = await _repo.InsertAsync(entidade);
        return CreatedAtAction(nameof(GetAll), new { id });
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
