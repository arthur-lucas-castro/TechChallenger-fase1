# Outputs centralizados de todo o projeto (VPC, EKS, RDS, ECR, IAM).

# --- VPC ---

output "vpc_id" {
  description = "ID da VPC criada."
  value       = module.vpc.vpc_id
}

output "private_subnet_ids" {
  description = "IDs das subnets privadas (nodes do EKS e RDS)."
  value       = module.vpc.private_subnets
}

output "public_subnet_ids" {
  description = "IDs das subnets públicas (load balancer)."
  value       = module.vpc.public_subnets
}

# --- EKS ---

output "eks_cluster_name" {
  description = "Nome do cluster EKS."
  value       = aws_eks_cluster.this.name
}

output "eks_cluster_endpoint" {
  description = "Endpoint da API do cluster EKS. Necessário para configurar o kubectl."
  value       = aws_eks_cluster.this.endpoint
}

output "eks_cluster_certificate_authority_data" {
  description = "Certificate authority (base64) do cluster EKS, usado para configurar o kubectl."
  value       = aws_eks_cluster.this.certificate_authority[0].data
}

output "eks_cluster_security_group_id" {
  description = "ID do security group primário do cluster EKS, usado pelos nodes (usado como origem permitida no SG do RDS)."
  value       = aws_eks_cluster.this.vpc_config[0].cluster_security_group_id
}

# --- RDS ---

output "rds_endpoint" {
  description = "Endpoint (host:porta) de conexão com o banco RDS."
  value       = aws_db_instance.this.endpoint
}

output "db_name" {
  description = "Nome do banco de dados no RDS."
  value       = aws_db_instance.this.db_name
}

output "db_username" {
  description = "Usuário administrador do RDS (sensível). Usado por infra/scripts/init-rds-schema.sh para aplicar o schema inicial."
  value       = var.db_username
  sensitive   = true
}

output "db_password" {
  description = "Senha do usuário administrador do RDS (sensível). Usado por infra/scripts/init-rds-schema.sh para aplicar o schema inicial."
  value       = var.db_password
  sensitive   = true
}

# --- ECR ---

output "ecr_repository_url" {
  description = "URL do repositório ECR, usada no docker build/push e no manifesto do Kubernetes."
  value       = aws_ecr_repository.app.repository_url
}

# --- IAM ---

output "lab_role_arn" {
  description = "ARN da LabRole (AWS Academy Learner Lab), reaproveitada pelo control plane e pelo node group do EKS em vez de roles criadas pelo Terraform."
  value       = data.aws_iam_role.lab_role.arn
}
