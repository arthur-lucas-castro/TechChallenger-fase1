# TechChallenger Fase 1

> Este README apresenta o Tech Challenge Fase, aqui está um descritivo de como rodar e as principais regras.
Os documentos do DDD estão no seguinte link:
https://app.notion.com/p/Documenta-o-Tech-Challenge-38ddf28d6d9d805cba3de5e50083f793?source=copy_link

---

## Regras de Negócio

### Validações de dados (Value Objects)

| Campo | Regra |
|---|---|
| `Dinheiro` | Valor não pode ser negativo |
| `Email` | Deve conter `@` e `.`; não pode ser vazio |
| `Telefone` | Deve ter 10 ou 11 dígitos numéricos |
| `Documento` | CPF (11 dígitos) ou CNPJ (14 dígitos) com dígito verificador válido; sequências repetidas são rejeitadas |
| `Placa` | Formato antigo `AAA9999` ou Mercosul `AAA9A99`; não pode ser vazia |

---

### Estoque (Catálogo)

- A quantidade informada para dar baixa deve ser maior que zero.
- Não é permitido dar baixa com quantidade superior ao estoque disponível.
- Não é possível dar baixa em uma peça que não possui estoque cadastrado.
- Ao adicionar estoque a uma peça já existente, o preço de custo médio é recalculado com média ponderada: `(qtdAtual × custoAtual + qtdNova × custoNovo) / (qtdAtual + qtdNova)`.
- Quando o estoque atual fica abaixo da quantidade mínima após uma baixa, um evento de alerta é disparado.

---

### Orçamento (Operação)

- Um orçamento só pode ser **aprovado** ou **recusado** se estiver com status `Pendente` ou `Enviado`.
- O preço total do orçamento é calculado automaticamente ao finalizar o diagnóstico: `Σ(preço × quantidade)` de todos os serviços e peças solicitados.

---

### Ordem de Serviço — Máquina de Estados (Operação)

O fluxo de status é estritamente controlado. Cada transição só é permitida a partir do status indicado:

```
Recebida → EmDiagnostico → AguardandoAprovacao → EmExecucao → Finalizada → Entregue
```

| Operação | Status requerido | Pré-condições adicionais |
|---|---|---|
| Iniciar diagnóstico | `Recebida` | — |
| Finalizar diagnóstico | `Recebida` ou `EmDiagnostico` | Cria orçamento e calcula preço total |
| Aprovar orçamento | `AguardandoAprovacao` | Orçamento deve existir |
| Recusar orçamento | `AguardandoAprovacao` | Orçamento deve existir |
| Iniciar execução | `AguardandoAprovacao` | Orçamento deve existir e estar com status `Aprovado` |
| Finalizar ordem | `EmExecucao` | — |
| Entregar veículo | `Finalizada` | — |

Qualquer tentativa de transição fora dessas regras lança `TransicaoStatusInvalidaException`.

---

### Execução de serviços (Operação)

- O status de execução de cada serviço solicitado segue: `Pendente → EmExecucao → Executado`.
- Só é possível alterar o status de um serviço que pertença à ordem em questão.

---

### Criação de Ordem de Serviço (Operação)

- Todos os serviços referenciados na criação da ordem devem existir no catálogo.
- Todas as peças referenciadas na criação da ordem devem existir no catálogo.

---

### Autenticação

- Senhas são armazenadas como hash BCrypt; a senha em texto plano nunca é persistida.
- O login retorna erro genérico tanto para usuário inexistente quanto para senha incorreta (sem distinção proposital).
- O token JWT gerado não possui refresh token; ao expirar, o usuário deve autenticar novamente.

---

## Libs Utilizadas

### Produção

| Biblioteca | Versão | Uso |
|---|---|---|
| `Microsoft.EntityFrameworkCore` | 8.0.0 | ORM principal |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 8.0.0 | Provider EF Core para PostgreSQL |
| `Npgsql` | 8.0.5 | Driver ADO.NET para PostgreSQL |
| `EFCore.NamingConventions` | 8.0.0 | Converte nomes de tabelas/colunas para minúsculas automaticamente |
| `Swashbuckle.AspNetCore` | 6.6.2 | Geração de documentação Swagger/OpenAPI |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.0 | Middleware de autenticação via JWT |
| `System.IdentityModel.Tokens.Jwt` | 8.0.0 | Geração e validação de tokens JWT |
| `BCrypt.Net-Next` | 4.0.3 | Hash de senhas com bcrypt |
| `Microsoft.Extensions.Logging.Abstractions` | 8.0.0 | Abstrações de logging para as camadas de aplicação |
| `Microsoft.Extensions.Configuration.Abstractions` | 8.0.0 | Abstrações de configuração |

### Testes

| Biblioteca | Versão | Uso |
|---|---|---|
| `xunit` | 2.9.3 | Framework de testes unitários |
| `xunit.runner.visualstudio` | 2.8.2 | Runner do xUnit integrado ao Visual Studio |
| `Moq` | 4.20.72 | Criação de mocks para isolamento de dependências |
| `coverlet.collector` | 6.0.4 | Coleta de cobertura de código (formato OpenCover) |
| `Microsoft.NET.Test.Sdk` | 17.12.0 | SDK de infraestrutura de testes .NET |

---

## Usuários padrão

Os seguintes usuários são criados automaticamente pelo script de inicialização do banco:

| Perfil | E-mail | Senha |
|---|---|---|
| Administrador | `adm@oficina.com` | `Adm@12345` |
| Funcionário | `funcionario@oficina.com` | `Func@12345` |

Use o endpoint `POST /Auth/login` com as credenciais acima para obter o token JWT.

---

## Como Executar

### Pré-requisitos

- Docker e Docker Compose
- .NET 8 SDK _(necessário apenas para análise Sonar)_
- PowerShell 5.1+ _(necessário apenas para análise Sonar)_

### Stack completa (API + banco de dados)

```bash
docker-compose up
```

- API: `http://localhost:5158`
- Swagger UI: `http://localhost:5158/swagger`

### Apenas o banco de dados

```bash
docker-compose up postgres
```

- Host: `localhost:5432`
- Banco: `techchallenger` | Usuário: `postgres` | Senha: `postgres`

### Modo desenvolvimento local (API sem Docker)

```bash
docker-compose up postgres
dotnet run --project src/Presentation/TechChallenger-fase1
```

---

## Análise com SonarQube

### 1. Subir o container

```bash
docker-compose -f docker-compose.sonar.yml up -d
```

### 2. Executar análise

```powershell
.\sonar-analyze.ps1
```

O script aguarda o SonarQube inicializar automaticamente (~1-2 min) e executa as seguintes etapas:

1. Aguarda o SonarQube ficar disponível
2. Configura a senha do admin
3. Cria o projeto
4. Gera token de análise
5. Instala `dotnet-sonarscanner` (se necessário)
6. Limpa artefatos anteriores
7. Inicia SonarScanner (`begin`)
8. Executa build + testes com cobertura
9. Envia resultados (`end`)

### 3. Acessar o dashboard

| Campo | Valor |
|-------|-------|
| URL | <http://localhost:9000/dashboard?id=techchallenger-fase1> |
| Login | `admin` |
| Senha | `Admin@sonar1` |
