# Backend remoto para o restante do projeto Terraform (tudo fora de bootstrap/).
#
# Os valores abaixo (bucket, dynamodb_table, region) vêm dos outputs de
# infra/bootstrap (state_bucket_name, lock_table_name, aws_region).
# O bloco "backend" NÃO aceita variáveis nem referências a outros recursos —
# os valores precisam estar hardcoded aqui ou ser passados via -backend-config
# no "terraform init" (veja o passo a passo no README/instruções do projeto).

terraform {
  backend "s3" {
    bucket         = "techchallenger-fase1-terraform-state-shared"
    key            = "techchallenger-fase1/terraform.tfstate"
    region         = "us-east-1"
    dynamodb_table = "techchallenger-fase1-terraform-lock-shared"
    encrypt        = true
  }
}
