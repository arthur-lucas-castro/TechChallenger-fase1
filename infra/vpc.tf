# VPC para o cluster EKS, subnets públicas/privadas, Internet Gateway e NAT Gateway.
#
# Usa o módulo oficial terraform-aws-modules/vpc/aws. As tags
# "kubernetes.io/cluster/<nome>", "kubernetes.io/role/elb" e
# "kubernetes.io/role/internal-elb" são exigidas pelo EKS/AWS Load Balancer
# Controller para descobrir automaticamente as subnets onde criar os
# load balancers públicos e internos.

module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "~> 5.0"

  name = "${var.cluster_name}-vpc"
  cidr = var.vpc_cidr

  azs             = var.availability_zones
  public_subnets  = var.public_subnet_cidrs
  private_subnets = var.private_subnet_cidrs

  enable_nat_gateway   = true
  single_nat_gateway   = var.single_nat_gateway
  enable_dns_hostnames = true
  enable_dns_support   = true

  public_subnet_tags = {
    "kubernetes.io/cluster/${var.cluster_name}" = "shared"
    "kubernetes.io/role/elb"                    = "1"
  }

  private_subnet_tags = {
    "kubernetes.io/cluster/${var.cluster_name}" = "shared"
    "kubernetes.io/role/internal-elb"           = "1"
  }

  tags = {
    Project     = var.cluster_name
    Environment = var.environment
    Terraform   = "true"
  }
}
