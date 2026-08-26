# Cluster EKS provisionado sobre a VPC definida em vpc.tf.
#
# Endpoint público habilitado para permitir kubectl local e acesso pela
# pipeline de CI/CD; endpoint privado habilitado para que os nodes (nas
# subnets privadas) alcancem a API sem sair pela internet.
#
# Conta AWS Academy Learner Lab: não é possível criar IAM roles novas, apenas
# usar as já existentes "LabRole" (role) e "LabInstanceProfile" (instance
# profile). Por isso a role do control plane e a do node group reutilizam a
# LabRole via data source, em vez de criar roles próprias.
data "aws_iam_role" "lab_role" {
  name = "LabRole"
}

# IMPORTANTE: este arquivo usa os recursos aws_eks_cluster/aws_eks_node_group
# diretamente, em vez do módulo terraform-aws-modules/eks/aws. O módulo (em
# qualquer versão, 19 a 21) declara internamente um data source
# "aws_iam_session_context" incondicional, usado para resolver a role de
# origem por trás da sessão STS assumida (para o bootstrap de admin do
# cluster e o key administrator do KMS). Isso faz uma chamada iam:GetRole na
# role "voclabs" — a role interna do AWS Academy usada para autenticar a
# sessão do Learner Lab — que tem "deny" explícito nessa conta. Como esse
# data source é criado sempre que o cluster existe (não dá para desabilitar
# via variável), a única forma de evitar o erro é não depender do módulo.
resource "aws_cloudwatch_log_group" "eks" {
  name              = "/aws/eks/${var.cluster_name}/cluster"
  retention_in_days = 7

  tags = {
    Project     = var.cluster_name
    Environment = var.environment
    Terraform   = "true"
  }
}

resource "aws_eks_cluster" "this" {
  name     = var.cluster_name
  role_arn = data.aws_iam_role.lab_role.arn
  version  = var.kubernetes_version

  vpc_config {
    subnet_ids              = module.vpc.private_subnets
    endpoint_public_access  = true
    endpoint_private_access = true
  }

  enabled_cluster_log_types = ["api", "audit"]

  depends_on = [aws_cloudwatch_log_group.eks]

  tags = {
    Project     = var.cluster_name
    Environment = var.environment
    Terraform   = "true"
  }
}

# Node group gerenciado dimensionado propositalmente enxuto para caber nas
# cotas do AWS Academy Learner Lab (máx. 9 instâncias EC2 e 32 vCPUs por
# região na conta toda). t3.micro = 2 vCPUs; com desired_size = 2 e
# max_size = 3, o consumo fica entre 4 e 6 vCPUs e 2-3 instâncias EC2,
# deixando folga para outros recursos da conta e ainda permitindo o HPA
# demonstrar escala de nodes até o teto de 3.
#
# Sem launch template customizado: as instâncias sobem com o security group
# primário do cluster (aws_eks_cluster.this.vpc_config[0].cluster_security_group_id),
# que é o que o SG do RDS libera em rds.tf.
#
# A LabRole precisa já ter as policies de worker node do EKS (AmazonEKSWorkerNodePolicy,
# AmazonEKS_CNI_Policy, AmazonEC2ContainerRegistryReadOnly ou equivalentes) —
# não é possível anexar policies novas a ela nesta conta.
resource "aws_eks_node_group" "default" {
  cluster_name    = aws_eks_cluster.this.name
  node_group_name = "default"
  node_role_arn   = data.aws_iam_role.lab_role.arn
  subnet_ids      = module.vpc.private_subnets

  instance_types = var.node_instance_types
  disk_size      = var.node_disk_size

  scaling_config {
    min_size     = var.node_group_min_size
    max_size     = var.node_group_max_size
    desired_size = var.node_group_desired_size
  }

  labels = {
    role = "app"
  }

  tags = {
    Project     = var.cluster_name
    Environment = var.environment
    Terraform   = "true"
  }
}
