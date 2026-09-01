# Manifestos Kubernetes (`/k8s`)

Este documento descreve o propósito de cada manifesto desta pasta. Os arquivos `.yaml` não contêm mais comentários — todo o racional foi centralizado aqui.

Existem dois conjuntos de manifestos:

- **`k8s/*.yaml`** — ambiente local (Docker Desktop / Minikube / Kind), com Postgres rodando como pod dentro do próprio cluster.
- **`k8s/aws/*.yaml`** — ambiente AWS (EKS + RDS), aplicado pela pipeline de CI/CD via `kubectl apply` (job `deploy-k8s`), onde o banco é o RDS gerenciado (não há pod de banco).

## Ambiente local (`k8s/`)

### `secret.yaml`

Dados sensíveis lidos pela aplicação: `ConnectionStrings__Default` (string de conexão completa com Postgres local, incluindo senha), `Jwt__SecretKey`, e as credenciais do Postgres (`POSTGRES_USER`, `POSTGRES_PASSWORD`) consumidas pelo pod do banco em `database.yaml`.

### `configmap.yaml`

Variáveis não sensíveis lidas pela aplicação (`Program.cs` / `appsettings.json`): ambiente ASP.NET Core, URLs, configuração de JWT (issuer/audience/expiração) e `POSTGRES_DB`.

Host, porta e nome do banco **não** aparecem espalhados aqui: o app lê uma única `ConnectionStrings__Default` já montada (ex.: `Host=postgres-service;Port=5432;Database=techchallenger;Username=postgres;Password=...`), que contém a senha e por isso fica inteira no Secret, não neste ConfigMap.

`POSTGRES_DB` é a exceção — não é sensível por si só, e é usado tanto pelo Postgres (`POSTGRES_DB`, no pod de `database.yaml`) quanto embutido na connection string do app (no Secret) — fica aqui para os dois consumirem o mesmo valor sem duplicar.

### `database.yaml`

PersistentVolumeClaim + Deployment + Service do Postgres para o ambiente local:

- **PVC** `postgres-pvc` — 1Gi, `ReadWriteOnce`.
- **Deployment** `postgres` — imagem `techchallenger-postgres:latest` (buildada localmente), estratégia `Recreate` (evita dois pods de banco escrevendo no mesmo volume simultaneamente). Recebe `POSTGRES_DB` do ConfigMap e `POSTGRES_USER`/`POSTGRES_PASSWORD` do Secret. A variável `PGDATA` aponta para um subdiretório (`/var/lib/postgresql/data/pgdata`) em vez da raiz do volume, para evitar erro de inicialização em provisioners cujo diretório raiz do volume já vem com arquivos (ex.: `lost+found`). `livenessProbe`/`readinessProbe` usam `pg_isready`.
- **Service** `postgres-service` — `ClusterIP` na porta 5432, usado como host na connection string do Secret da API.

### `deployment.yaml`

Deployment da API para o ambiente local: 2 réplicas, imagem `techchallenger-fase1:latest` (buildada localmente, `imagePullPolicy: IfNotPresent`), variáveis injetadas via `configmap.yaml` + `secret.yaml`, probes de liveness/readiness batendo em `/health` na porta 8080.

### `service.yaml`

Expõe a API localmente via `NodePort` na porta 8080.

## Ambiente AWS (`k8s/aws/`)

### `secret.yaml`

Equivalente ao Secret local, mas a `ConnectionStrings__Default` aponta para o endpoint do RDS (placeholders `<rds-endpoint>`, `<rds-username>`, `<rds-password>`, substituídos pela pipeline de CI/CD a partir dos outputs do Terraform). Não contém `POSTGRES_USER`/`POSTGRES_PASSWORD` porque não há pod de banco neste ambiente — quem gerencia essas credenciais é o RDS.

### `configmap.yaml`

Mesmas variáveis não sensíveis do ambiente local (ambiente ASP.NET Core, URLs, configuração de JWT), sem `POSTGRES_DB` — o nome do banco já vem embutido na connection string do Secret, e não há pod de Postgres para consumi-lo separadamente.

### `deployment.yaml`

Deployment da API para o ambiente AWS: 2 réplicas, `image: IMAGE_PLACEHOLDER` (substituído pela pipeline de CI/CD pela URL da imagem no ECR), `imagePullPolicy: Always` (garante que a tag mais recente publicada seja sempre puxada). Mesmas probes de `/health` do ambiente local.

Os `resources.requests`/`limits` (100m/128Mi de request, 250m/256Mi de limit) são mantidos iguais ao ambiente local porque já são enxutos e cabem confortavelmente em um node `t3.micro` (1 vCPU / 1GiB RAM). Referências de dimensionamento para `t3.micro`:

- O allocatable real fica em ~700-800Mi de memória e ~940m de CPU depois do `kube-reserved`/`system-reserved` da AMI do EKS (o restante é consumido por `kubelet`, `kube-proxy`, `aws-node`/VPC CNI, etc.).
- 2 réplicas × 128Mi/100m de request = 256Mi/200m no total, o que cabe em um único `t3.micro` junto com os daemonsets do sistema — mas para ter HA de verdade (sobreviver à perda de 1 node) é necessário ter pelo menos 2 nodes no node group, não só 2 réplicas.
- Os limits (256Mi/250m) evitam que um pod estoure a memória do node sozinho; se no futuro for adicionado HPA ou mais pods no mesmo node, revisar esses números com `kubectl top nodes/pods`.

### `hpa.yaml`

HorizontalPodAutoscaler para o Deployment da API: entre 2 e 5 réplicas, escalando por utilização média de CPU (60%) ou memória (70%). Funciona em conjunto com o `node_group_max_size = 3` de `infra/eks.tf`, que permite o node group crescer para acomodar as réplicas extras.

### `metrics-server.yaml`

Pré-requisito do HPA: sem ele, o HPA fica com os targets em `<unknown>` para sempre e nunca escala, mesmo sob carga real. O EKS não vem com metrics-server instalado por padrão — por isso o manifest oficial (`kubernetes-sigs/metrics-server`, vendorizado aqui na versão `v0.9.0` em vez de apontar pra URL `latest`, pra manter reprodutibilidade) é aplicado pelo `deploy-k8s` do `ci-cd.yml` logo antes do `hpa.yaml`. Se a infra/cluster for recriado do zero, esse passo garante que o autoscale volte a funcionar sem intervenção manual.



### `service.yaml`

Expõe a API publicamente via um Network Load Balancer da AWS (`type: LoadBalancer` + annotation `service.beta.kubernetes.io/aws-load-balancer-type: nlb`), criado pelo AWS Load Balancer Controller do EKS. Este Service é aplicado apenas via `kubectl` (pipeline de CI/CD) e não existe no state do Terraform — por isso `infra/eks.tf` tem um `null_resource` (`cleanup_k8s_load_balancers`) dedicado a remover esse Service e aguardar o NLB sumir da VPC antes de um `terraform destroy` tentar apagar as subnets.
