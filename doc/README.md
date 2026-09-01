# TechChallenger — Fase 1

API REST para gestão de uma oficina mecânica (atendimento de clientes/veículos, catálogo de peças e serviços, e o ciclo completo de ordens de serviço), construída em **ASP.NET Core 8** com **Domain-Driven Design (DDD)**, **EF Core 8 + PostgreSQL**, conteinerizada com **Docker** e implantada em **Kubernetes (Amazon EKS)** com infraestrutura provisionada via **Terraform**.

---

## Sumário

- [Objetivos desta fase](#objetivos-desta-fase)
- [Arquitetura da solução](#arquitetura-da-solução)
  - [Componentes da aplicação](#componentes-da-aplicação)
  - [Infraestrutura provisionada](#infraestrutura-provisionada)
  - [Fluxo de deploy](#fluxo-de-deploy)
- [Execução local](#execução-local)
- [Deploy em Kubernetes](#deploy-em-kubernetes)
- [Provisionamento da infraestrutura com Terraform](#provisionamento-da-infraestrutura-com-terraform)

---

## Objetivos desta fase

Esta fase do Tech Challenge evolui a API (já modelada em DDD com bounded contexts, construída nas fases anteriores) para um cenário de **produção em nuvem**, com foco em:

- **Conteinerização** da aplicação e do banco de dados.
- **Orquestração via Kubernetes**, com deployments, services, config/secrets e autoscaling horizontal (HPA) baseado em CPU/memória.
- **Infraestrutura como código (Terraform)**, provisionando de ponta a ponta a rede (VPC), o cluster Kubernetes gerenciado (EKS), o banco de dados gerenciado (RDS) e o registro de imagens (ECR) na AWS.
- **Pipeline de CI/CD (GitHub Actions)**, cobrindo build, testes automatizados, publicação da imagem e deploy no cluster de forma automatizada e auditável.

O ambiente-alvo é uma conta **AWS Academy Learner Lab**, o que impõe restrições específicas (sem criação de IAM roles novas, cotas baixas de EC2/vCPU, credenciais temporárias) refletidas nas decisões de infraestrutura — detalhadas em [INFRA.md](INFRA.md).

---

## Arquitetura da solução

### Componentes da aplicação

A API segue DDD com bounded contexts isolados, todos sob `src/`:

| Bounded Context | Responsabilidade |
|---|---|
| **Atendimento** | Cadastro de clientes e veículos |
| **Catalogo** | Peças, serviços e controle de estoque |
| **Operacao** | Ordens de serviço, orçamentos e sua máquina de estados |
| **Compartilhado** | Entidade base, value objects comuns (`Dinheiro`, `Email`, `Telefone`…), `AppDbContext`, `BaseRepository<T>`, despacho de domain events e envio de e-mail (MailKit/SMTP) |

Cada contexto segue o fluxo de camadas `Controller → IService → IRepositorio`, com comunicação entre camadas sempre via DTOs (nunca entidades de domínio expostas ao controller).

```
┌─────────────────────────────────────────────────────────┐
│                     Presentation                          │
│         Controllers (Atendimento/Catalogo/Operacao)       │
└───────────────────────────┬─────────────────────────────┘
                             │ DTOs
┌───────────────────────────▼─────────────────────────────┐
│                       Application                          │
│           Services · DTOs · Domain Event Handlers          │
└───────────────────────────┬─────────────────────────────┘
                             │ Entidades de domínio
┌───────────────────────────▼─────────────────────────────┐
│                          Domain                             │
│     Aggregate Roots · Value Objects · Domain Events          │
└───────────────────────────┬─────────────────────────────┘
                             │
┌───────────────────────────▼─────────────────────────────┐
│                      Infrastructure                         │
│     Repositórios (EF Core) · AppDbContext · Email (SMTP)    │
└───────────────────────────┬─────────────────────────────┘
                             │
                      PostgreSQL (RDS em produção)
```

Outros componentes de apoio:

- **Mailpit** — servidor SMTP de testes usado no ambiente local para capturar os e-mails de notificação enviados a cada mudança de status da ordem de serviço.
- **Autenticação JWT** — login via `POST /Auth/login`, com senhas em hash BCrypt.

### Infraestrutura provisionada

Provisionada via Terraform ([infra/](../infra), detalhes em [INFRA.md](INFRA.md)):

```
                        AWS (us-east-1)
┌──────────────────────────────────────────────────────────────────┐
│  VPC (10.0.0.0/16)                                                 │
│                                                                      │
│  ┌─── Subnets públicas ───┐        ┌─── Subnets privadas ───┐      │
│  │  Internet Gateway       │        │                          │      │
│  │  NAT Gateway             │◄──────┤  EKS Node Group          │      │
│  │  Network Load Balancer   │       │  (t3.micro, 1-3 nodes)   │      │
│  │       ▲                  │        │       │                  │      │
│  └───────┼──────────────────┘        │       ▼                  │      │
│          │                            │  RDS PostgreSQL          │      │
│  Internet │                            │  (db.t3.micro)           │      │
│          │                            └──────────────────────────┘      │
└──────────┼──────────────────────────────────────────────────────────┘
           │
     Usuário / kubectl / pipeline CI-CD

  ECR (registro da imagem Docker da API) ── consumido pelo EKS
```

- **VPC** — `terraform-aws-modules/vpc/aws`, com subnets públicas (load balancer) e privadas (nodes EKS + RDS) em 3 AZs, Internet Gateway e NAT Gateway.
- **EKS** — cluster Kubernetes gerenciado, node group dimensionado para as cotas do Learner Lab (`t3.micro`, entre 1 e 3 nodes), reaproveitando a `LabRole` já existente na conta em vez de criar IAM roles novas.
- **RDS (PostgreSQL)** — instância gerenciada em subnet privada, sem exposição pública, acessível apenas pelo security group do cluster EKS.
- **ECR** — repositório da imagem Docker da API, com scan de vulnerabilidades e lifecycle mantendo as últimas 10 imagens.
- **Backend remoto do state** ([infra/bootstrap/](../infra/bootstrap)) — bucket S3 (versionado, criptografado) + tabela DynamoDB de lock, provisionados uma única vez e usados como backend do restante do projeto.

Todas as decisões de design (por que EKS gerenciado sem o módulo oficial, dimensionamento dos nodes, notificação por e-mail sem SES, cleanup do Load Balancer no destroy, etc.) estão documentadas em [INFRA.md](INFRA.md).

### Fluxo de deploy

Automatizado via GitHub Actions ([.github/workflows/](../.github/workflows)):

```
1. Pull Request  ──────────────►  ci-cd.yml: build-and-test
                                   (dotnet build + dotnet test)

2. workflow_dispatch em branch    ci-cd.yml (confirm_deploy=yes)
   release/X.Y.Z          │
                           ├──► build-and-test
                           ├──► build-and-push-image
                           │       dotnet publish → docker build → push no ECR
                           ├──► deploy-k8s
                           │       kubectl apply: configmap, secret, deployment,
                           │       service, metrics-server, hpa
                           │       kubectl rollout status (aguarda o rollout)
                           └──► merge-to-master
                                   PR automático release → master (auto-merge)

  Infraestrutura (sob demanda, workflows separados):
  infra-apply.yml    → terraform plan/apply (provisiona VPC/EKS/RDS/ECR)
  infra-destroy.yml  → terraform plan-destroy → aprovação manual → destroy
  database-migration.yml → aplica database/init/01_schema.sql no RDS via Job k8s
```

Pontos-chave:

- O deploy de aplicação (`ci-cd.yml`) e o provisionamento de infraestrutura (`infra-apply.yml`) são **pipelines independentes** — a infraestrutura só precisa ser reaplicada quando algo em `infra/` muda.
- Segredos de runtime (`ConnectionStrings__Default`, `Jwt__SecretKey`, credenciais SMTP) são aplicados no cluster a partir de um único GitHub Actions secret (`K8S_SECRET_ENV`, formato `.env`), nunca commitados.
- `infra-destroy.yml` exige usuário autorizado, confirmação dupla (`confirm_destroy=yes` + digitar `DESTROY`) e aprovação manual de ambiente antes de destruir qualquer recurso.

---

## Execução local

### Pré-requisitos

- Docker e Docker Compose
- .NET 8 SDK _(opcional, apenas para rodar a API fora de container ou para a análise SonarQube)_

### Stack completa (API + banco + mailpit)

```bash
docker-compose up
```

- API: `http://localhost:5158`
- Swagger UI: `http://localhost:5158/swagger`
- Mailpit (captura de e-mails de notificação): `http://localhost:8025`

### Apenas o banco de dados

```bash
docker-compose up postgres
```

- Host: `localhost:5432` · Banco: `techchallenger` · Usuário: `postgres` · Senha: `postgres`

### Modo desenvolvimento (API sem Docker)

```bash
docker-compose up postgres mailpit
dotnet run --project src/Presentation/TechChallenger-fase1
```

### Testes automatizados

```bash
dotnet test src/TechChallenger-fase1.sln --filter "FullyQualifiedName~Tests"
```

---

## Deploy em Kubernetes

Manifestos em [k8s/](../k8s) (detalhes de cada arquivo em [K8S.md](K8S.md)). Existem dois conjuntos:

- **`k8s/*.yaml`** — ambiente local (Docker Desktop, Minikube ou Kind), com Postgres rodando como pod dentro do próprio cluster.
- **`k8s/aws/*.yaml`** — ambiente AWS (EKS + RDS gerenciado), aplicado automaticamente pela pipeline `ci-cd.yml` (job `deploy-k8s`).

### Local (Docker Desktop / Minikube / Kind)

```bash
# build das imagens usadas pelos manifestos locais
docker build -t techchallenger-fase1:latest .
docker build -t techchallenger-postgres:latest ./database

kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/database.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
kubectl apply -f k8s/mailpit.yaml

kubectl rollout status deployment/techchallenger-api
```

- API exposta via `NodePort` na porta `8080` do node.

### AWS (EKS) — manual

Normalmente aplicado pela pipeline (`ci-cd.yml`), mas pode ser reproduzido manualmente após o `terraform apply` (ver seção seguinte) e configuração do `kubectl`:

```bash
aws eks update-kubeconfig --name <eks_cluster_name> --region us-east-1

# substituir IMAGE_PLACEHOLDER pela URL real da imagem no ECR antes de aplicar
sed -i "s#IMAGE_PLACEHOLDER#<ecr_repository_url>:<tag>#" k8s/aws/deployment.yaml

kubectl apply -f k8s/aws/configmap.yaml
kubectl apply -f k8s/aws/secret.yaml     # preencher com o endpoint real do RDS antes
kubectl apply -f k8s/aws/deployment.yaml
kubectl apply -f k8s/aws/service.yaml
kubectl apply -f k8s/aws/metrics-server.yaml   # pré-requisito do HPA
kubectl apply -f k8s/aws/hpa.yaml

kubectl rollout status deployment/techchallenger-api --timeout=180s
kubectl get service techchallenger-api -o wide   # URL do Load Balancer (NLB)
```

- API exposta publicamente via **Network Load Balancer** (AWS Load Balancer Controller do EKS).
- **HPA** escala entre 2 e 5 réplicas por CPU (60%) ou memória (70%), em conjunto com o autoscaling do node group (até 3 nodes).

Na pipeline (`ci-cd.yml`), esse fluxo roda de forma automatizada disparando o workflow manualmente (`workflow_dispatch`) a partir de uma branch `release/X.Y.Z`, com `confirm_deploy = yes`.

---

## Provisionamento da infraestrutura com Terraform

Terraform em [infra/](../infra), com o rationale completo de cada arquivo em [INFRA.md](INFRA.md).

### 1. Backend remoto (uma única vez por conta AWS)

```bash
cd infra/bootstrap
terraform init
terraform apply
terraform output   # copiar bucket/tabela para infra/backend.tf, se ainda não preenchidos
```

### 2. Infraestrutura principal (VPC, EKS, RDS, ECR)

```bash
cd infra
cp terraform.tfvars.example terraform.tfvars
# editar terraform.tfvars: preencher ao menos db_username e db_password

terraform init
terraform plan
terraform apply
```

Outputs relevantes após o `apply` (usados para configurar o deploy):

| Output | Uso |
|---|---|
| `ecr_repository_url` | Secret `ECR_REPOSITORY_URL` da pipeline `ci-cd.yml` |
| `eks_cluster_name` | Secret `EKS_CLUSTER_NAME`, e `aws eks update-kubeconfig` |
| `rds_endpoint` | Host usado no Secret `DB_HOST` / `ConnectionStrings__Default` |

### 3. Aplicar o schema inicial no RDS

O RDS não tem exposição pública, então o schema é aplicado via um pod temporário dentro do cluster:

```bash
./infra/scripts/init-rds-schema.sh
```

(via pipeline, o mesmo é feito por `database-migration.yml`, como um Kubernetes Job).

### 4. Destruir a infraestrutura

```bash
cd infra
terraform destroy
```

> Via pipeline, use `infra-destroy.yml` — exige usuário autorizado, dupla confirmação e aprovação manual antes de aplicar.

### Via GitHub Actions (recomendado)

Os workflows `infra-apply.yml` e `infra-destroy.yml` executam os mesmos passos (`terraform plan`/`apply`/`destroy`) usando as credenciais temporárias do AWS Academy Learner Lab configuradas nos secrets do ambiente `aws-deploy`, sem necessidade de rodar Terraform localmente.
