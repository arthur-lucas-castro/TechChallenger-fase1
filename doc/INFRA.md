# Infraestrutura Terraform (`/infra`)

Este documento descreve o propósito e as decisões de design de cada arquivo desta pasta. Os arquivos `.tf`, `.tfvars` e `.sh` não contêm mais comentários — todo o racional foi centralizado aqui.

Provisiona um cluster **EKS** (Kubernetes gerenciado), uma instância **RDS Postgres** e um repositório **ECR**, todos dentro de uma **VPC** dedicada, para rodar a API do TechChallenger na AWS. O projeto roda em uma conta **AWS Academy Learner Lab**, o que impõe várias restrições (sem criação de IAM roles novas, cotas baixas de EC2/vCPU) refletidas nas decisões abaixo.

## Backend remoto do state

### `backend.tf`

Configura o backend remoto (`s3`) usado pelo restante do projeto (tudo fora de `bootstrap/`). Os valores de `bucket`, `dynamodb_table` e `region` vêm dos outputs de `infra/bootstrap` (`state_bucket_name`, `lock_table_name`, `aws_region`).

O bloco `backend` do Terraform **não aceita variáveis nem referências a outros recursos** — por isso os valores estão hardcoded aqui. Alternativamente, podem ser passados via `-backend-config` no `terraform init`.

### `bootstrap/` — recursos que sustentam o backend remoto

Esta subpasta é a **única do projeto sem backend remoto**: ela cria os próprios recursos que o backend remoto (`backend.tf`) vai usar (bucket S3 e tabela DynamoDB), então o state dela fica local (`terraform.tfstate`, versionado nesta pasta).

- **`main.tf`** — cria o bucket S3 (`aws_s3_bucket_versioning`, criptografia SSE-AES256, bloqueio de acesso público) e a tabela DynamoDB (`terraform_lock`) usada para lock de state. O bucket tem `lifecycle { prevent_destroy = true }` para evitar que um `terraform destroy` acidental apague o state de todo o restante do projeto.
- **`variables.tf`** — `aws_region`, `project_name`, `environment` (usados para nomear os recursos de backend).
- **`outputs.tf`** — expõe `state_bucket_name`, `state_bucket_arn`, `lock_table_name`, `aws_region`, consumidos manualmente para preencher `backend.tf`.
- **`versions.tf`** — pin do provider AWS (`~> 5.0`) e da versão mínima do Terraform (`>= 1.5.0`).

## VPC

### `vpc.tf`

Cria a VPC, subnets públicas/privadas, Internet Gateway e NAT Gateway usando o módulo oficial `terraform-aws-modules/vpc/aws`.

As tags `kubernetes.io/cluster/<nome>`, `kubernetes.io/role/elb` e `kubernetes.io/role/internal-elb` são exigidas pelo EKS / AWS Load Balancer Controller para descobrir automaticamente em quais subnets criar load balancers públicos e internos.

## EKS

### `eks.tf`

- **`data.aws_iam_role.lab_role`** — reaproveita a role `LabRole` já existente na conta AWS Academy (não é possível criar IAM roles novas nessa conta; só é permitido usar `LabRole` e `LabInstanceProfile`). Tanto o control plane quanto o node group usam essa mesma role.
- **Por que os recursos `aws_eks_cluster` / `aws_eks_node_group` são usados diretamente, em vez do módulo `terraform-aws-modules/eks/aws`**: o módulo (em qualquer versão de 19 a 21) declara internamente um data source `aws_iam_session_context` incondicional, usado para resolver a role de origem por trás da sessão STS assumida. Isso dispara uma chamada `iam:GetRole` na role `voclabs` (role interna do AWS Academy usada para autenticar a sessão do Learner Lab), que tem **deny explícito** nessa conta. Como esse data source é criado sempre que o cluster existe — não há flag para desabilitá-lo — a única forma de evitar o erro é não depender do módulo.
- **`aws_eks_cluster.this`** — endpoint público habilitado (permite `kubectl` local e acesso pela pipeline de CI/CD) e endpoint privado habilitado (nodes nas subnets privadas alcançam a API sem sair pela internet).
- **`aws_eks_node_group.default`** — dimensionado propositalmente enxuto para caber nas cotas do AWS Academy Learner Lab (máx. 9 instâncias EC2 e 32 vCPUs por região na conta toda). `t3.micro` = 2 vCPUs; com `desired_size = 2` e `max_size = 3`, o consumo fica entre 4 e 6 vCPUs e 2-3 instâncias EC2, deixando folga para outros recursos da conta e ainda permitindo o HPA demonstrar escala de nodes até o teto de 3. Não usa launch template customizado: as instâncias sobem com o security group primário do cluster, que é o mesmo liberado pelo SG do RDS em `rds.tf`. A `LabRole` precisa já ter as policies de worker node do EKS (`AmazonEKSWorkerNodePolicy`, `AmazonEKS_CNI_Policy`, `AmazonEC2ContainerRegistryReadOnly` ou equivalentes) — não é possível anexar policies novas a ela nesta conta.
- **`null_resource.cleanup_k8s_load_balancers`** — faz o cleanup do Network Load Balancer criado pelo Kubernetes fora do Terraform. `k8s/aws/service.yaml` (type `LoadBalancer`, annotation `aws-load-balancer-type: nlb`) é aplicado via `kubectl apply` pelo job `deploy-k8s` do CI/CD — nunca pelo Terraform. Em resposta, o AWS Load Balancer Controller do EKS cria um NLB com ENIs anexadas às subnets públicas da VPC. Como esse NLB não existe no state do Terraform, um `terraform destroy` tenta apagar as subnets antes de o NLB (e suas ENIs) serem removidos, e falha com `DependencyViolation: has dependencies and cannot be deleted`. Este `null_resource` roda, só no momento do destroy, um `kubectl delete service` para remover o LoadBalancer e espera as ENIs sumirem da VPC antes de deixar o destroy prosseguir para o node group / cluster / VPC. Provisioners `when = destroy` só podem referenciar `self` (não outros resources), por isso os valores necessários vão em `triggers`; o `depends_on` garante que este resource seja destruído **antes** do cluster e da VPC (ordem de destroy é o inverso da ordem de dependência), com o cluster ainda no ar para o `kubectl` conseguir falar com ele.

## RDS

### `rds.tf`

Instância RDS (Postgres) na VPC definida em `vpc.tf`. Fica em subnets privadas, sem exposição pública, e só aceita conexões vindas do security group primário do cluster EKS (o node group não usa launch template customizado, então os nodes sobem com esse SG).

`skip_final_snapshot = true` porque este é um ambiente de estudo/challenge, evitando reter snapshot final ao destruir. Em produção, trocar para `skip_final_snapshot = false` e informar `final_snapshot_identifier`.

## ECR

### `ecr.tf`

Repositório ECR para a imagem Docker da aplicação, com scan automático na publicação (`scan_on_push`) e política de lifecycle que mantém apenas as últimas 10 imagens.

`force_delete = true`: sem essa opção, `terraform destroy` falha com `RepositoryNotEmptyException` sempre que houver imagens publicadas (job `build-and-push-image` do `ci-cd.yml`) — `force_delete` manda apagar as imagens junto com o repositório.

## Variáveis e outputs

### `variables.tf`

Declara todas as variáveis de entrada (VPC, EKS, RDS, ECR). As descrições de cada variável (`description = "..."`) já documentam seu propósito e continuam no próprio arquivo — não são comentários, fazem parte do schema do Terraform e alimentam `terraform plan`/`terraform-docs`. Pontos que merecem destaque:

- `kubernetes_version` e `db_engine_version` têm `default = null` propositalmente: deixando null, o EKS/RDS usa a versão padrão disponível no momento do apply, evitando erros de "versão não suportada" quando uma versão antiga sai de suporte.
- `node_instance_types` (default `t3.micro`) e `node_disk_size` (default 20GB, mínimo exigido pela AMI `AL2023_x86_64_STANDARD`) refletem as cotas do AWS Academy Learner Lab.
- `db_allocated_storage`/`db_max_allocated_storage` ficam bem abaixo do limite de 100GB por volume EBS do Learner Lab.

### `outputs.tf`

Centraliza os outputs de todo o projeto, agrupados por área: VPC, EKS, RDS, ECR e IAM. Destaques:

- `db_username`/`db_password` são marcados `sensitive = true` e usados por `infra/scripts/init-rds-schema.sh` para aplicar o schema inicial.
- `eks_cluster_security_group_id` é o SG usado como origem permitida no SG do RDS.
- `lab_role_arn` documenta que a `LabRole` (AWS Academy Learner Lab) é reaproveitada pelo control plane e pelo node group do EKS, em vez de roles criadas pelo Terraform.

### `versions.tf`

Pin do provider AWS (`~> 5.0`) e da região (`us-east-1`), e versão mínima do Terraform (`>= 1.5.0`).

## Variáveis de exemplo

### `terraform.tfvars.example`

Modelo de arquivo de variáveis. Deve ser copiado para `terraform.tfvars` (ignorado pelo git) e os valores ajustados — principalmente `db_username` e `db_password`, que não têm default e não devem ser commitados:

```bash
cp terraform.tfvars.example terraform.tfvars
```

`kubernetes_version` e `db_engine_version` aparecem comentados nos exemplos originais — a orientação é **não** definir esses valores (deixar sem entrada), para que EKS/RDS usem a versão padrão vigente no momento do apply, evitando erros de versão descontinuada.

### `terraform.tfvars`

Valores reais usados neste ambiente (contém a senha do banco — não deveria ser commitado em um cenário real; aqui está presente por ser um ambiente de estudo/challenge).

## Scripts

### `scripts/init-rds-schema.sh`

Aplica o schema inicial (`database/init/01_schema.sql`) na instância RDS. Como o RDS não tem exposição pública, o script sobe um pod temporário (`pg-init`, imagem `postgres:16-alpine`) dentro do cluster EKS, copia o arquivo de schema para dentro do pod via `kubectl cp` e roda `psql` a partir dele, depois remove o pod.

Se o pod não conseguir ser agendado a tempo (o tipo de instância dos nodes impõe um limite de pods/nó via VPC CNI — 4 no `t3.micro` default, 11 se o apply usar `t3.small` via `node_instance_type` do workflow `infra-apply.yml`), o script orienta a reduzir temporariamente as réplicas da API (`kubectl scale deployment techchallenger-api --replicas=1`), rodar o script novamente e depois devolver as réplicas.

## Lock file

### `.terraform.lock.hcl`

Gerado automaticamente pelo `terraform init` — fixa os hashes exatos dos providers usados. Não deve ser editado manualmente.

## Notificação por e-mail — por que não Amazon SES

A API envia e-mail de notificação ao cliente a cada mudança de status da Ordem de Serviço (ver `Compartilhado.Infrastructure.Email`, implementação via MailKit/SMTP). Essa implementação é **agnóstica de provedor** (host/porta/usuário/senha configuráveis via o Secret `techchallenger-api-secret`), propositalmente **sem usar Amazon SES**: SES em modo produção exige sair do sandbox (abrir caso de suporte ou verificar domínio com acesso total à conta) e credenciais SMTP dedicadas via IAM — nenhuma das duas coisas é viável na conta AWS Academy Learner Lab (sem criação de IAM roles/usuários novos, sem acesso a suporte AWS). Em vez disso, o secret é preenchido com credenciais de um provedor SMTP externo (Gmail com senha de app, na configuração atual) — trocar de provedor no futuro (para SES ou outro) é só uma mudança de valores no secret, sem alterar código.

O Secret `techchallenger-api-secret` (`ConnectionStrings__Default`, `Jwt__SecretKey`, `Email__Host`, `Email__Username`, `Email__Password`) é gerado e aplicado automaticamente pelo job `deploy-k8s` da pipeline (`.github/workflows/ci-cd.yml`), a partir de um **único** GitHub Actions Environment Secret do ambiente `aws-deploy` chamado `K8S_SECRET_ENV`. Esse secret guarda o conteúdo no formato `.env` (uma linha `Chave=Valor` por variável, ex.:
```
ConnectionStrings__Default=Host=...;Port=5432;Database=...;Username=...;Password=...
Jwt__SecretKey=...
Email__Host=smtp.gmail.com
Email__Username=...
Email__Password=...
```
), que a pipeline grava num arquivo temporário e aplica com `kubectl create secret generic ... --from-env-file=... --dry-run=client -o yaml | kubectl apply -f -`. Adicionar/rotacionar uma credencial é só editar o texto desse único secret no GitHub — não precisa mexer no workflow. O arquivo `k8s/aws/secret.yaml` (fora do controle de versão) deixou de ser necessário para o deploy via pipeline — serve só como referência local ou para aplicação manual em um cenário de emergência fora da pipeline.

## Pastas não versionadas manualmente

- **`.terraform/`** — cache local de providers e módulos baixados pelo `terraform init` (inclui o módulo vendorizado `terraform-aws-modules/eks/aws` usado só como referência, mas não consumido diretamente por `eks.tf` — ver seção EKS acima). Não é código do projeto e é recriada a qualquer momento.
