terraform {
  required_version = ">= 1.5.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }

  # Esta pasta é intencionalmente a única do projeto SEM backend remoto:
  # ela cria os próprios recursos que o backend remoto vai usar (bucket
  # S3 e tabela DynamoDB), então o state dela fica local (terraform.tfstate).
}

provider "aws" {
  region = var.aws_region
}
