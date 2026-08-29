#!/usr/bin/env bash
# Aplica database/init/01_schema.sql no RDS a partir de dentro do cluster EKS.
#
# Diferente do Postgres local (que roda o script de inicialização sozinho via
# docker-entrypoint-initdb.d), o RDS sobe vazio. Rode este script uma vez
# depois do primeiro "terraform apply" (ou sempre que o RDS for recriado do
# zero) para criar as tabelas e os usuários seed.
#
# O RDS não é publicamente acessível (só aceita conexões vindas do security
# group dos nodes do EKS), por isso o script sobe um pod temporário dentro do
# cluster para aplicar o schema.
#
# Pré-requisitos:
#   - kubectl configurado apontando para o cluster certo:
#       aws eks update-kubeconfig --region us-east-1 --name $(terraform -chdir=infra output -raw eks_cluster_name)
#   - infra/terraform.tfvars preenchido e "terraform apply" já rodado
#
# Uso: ./infra/scripts/init-rds-schema.sh

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
SCHEMA_FILE="$REPO_ROOT/database/init/01_schema.sql"

RDS_HOST=$(terraform -chdir="$REPO_ROOT/infra" output -raw rds_endpoint | cut -d: -f1)
DB_USER=$(terraform -chdir="$REPO_ROOT/infra" output -raw db_username)
DB_PASS=$(terraform -chdir="$REPO_ROOT/infra" output -raw db_password)
DB_NAME=$(terraform -chdir="$REPO_ROOT/infra" output -raw db_name)

echo "Aplicando $SCHEMA_FILE em ${DB_NAME}@${RDS_HOST}..."

kubectl delete pod pg-init --ignore-not-found >/dev/null
kubectl run pg-init --image=postgres:16-alpine --restart=Never --command -- sleep 3600

if ! kubectl wait --for=condition=Ready pod/pg-init --timeout=60s; then
  echo
  echo "pg-init não conseguiu ser agendado a tempo."
  echo "Nos nodes t3.micro (limite de 4 pods/nó do VPC CNI), pode faltar espaço."
  echo "Abra espaço temporariamente e rode o script de novo:"
  echo "  kubectl scale deployment techchallenger-api --replicas=1"
  echo "  ./infra/scripts/init-rds-schema.sh"
  echo "Depois, devolva as réplicas:"
  echo "  kubectl scale deployment techchallenger-api --replicas=2"
  exit 1
fi

MSYS_NO_PATHCONV=1 kubectl cp "$SCHEMA_FILE" pg-init:/tmp/01_schema.sql
MSYS_NO_PATHCONV=1 kubectl exec pg-init -- psql \
  "postgresql://${DB_USER}:${DB_PASS}@${RDS_HOST}:5432/${DB_NAME}" \
  -f /tmp/01_schema.sql

kubectl delete pod pg-init

echo "Schema aplicado com sucesso."
