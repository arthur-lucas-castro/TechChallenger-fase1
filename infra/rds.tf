# Instância RDS (Postgres) na VPC definida em vpc.tf.
#
# Fica em subnets privadas, sem exposição pública, e só aceita conexões
# vindas do security group primário do cluster EKS (o node group não usa
# launch template customizado, então os nodes sobem com esse SG).

resource "aws_db_subnet_group" "this" {
  name       = "${var.cluster_name}-db"
  subnet_ids = module.vpc.private_subnets

  tags = {
    Project     = var.cluster_name
    Environment = var.environment
    Terraform   = "true"
  }
}

resource "aws_security_group" "rds" {
  name        = "${var.cluster_name}-rds"
  description = "Permite acesso ao RDS apenas a partir dos nodes do EKS"
  vpc_id      = module.vpc.vpc_id

  ingress {
    description     = "Acesso ao banco a partir dos nodes do EKS"
    from_port       = var.db_port
    to_port         = var.db_port
    protocol        = "tcp"
    security_groups = [aws_eks_cluster.this.vpc_config[0].cluster_security_group_id]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Project     = var.cluster_name
    Environment = var.environment
    Terraform   = "true"
  }
}

resource "aws_db_instance" "this" {
  identifier = "${var.cluster_name}-db"

  engine         = var.db_engine
  engine_version = var.db_engine_version
  instance_class = var.db_instance_class
  port           = var.db_port

  db_name  = var.db_name
  username = var.db_username
  password = var.db_password

  allocated_storage     = var.db_allocated_storage
  max_allocated_storage = var.db_max_allocated_storage
  storage_type          = "gp3"
  storage_encrypted     = true

  db_subnet_group_name   = aws_db_subnet_group.this.name
  vpc_security_group_ids = [aws_security_group.rds.id]
  publicly_accessible    = false

  backup_retention_period = var.db_backup_retention_period
  backup_window           = "03:00-04:00"
  maintenance_window      = "mon:04:30-mon:05:30"

  # Ambiente de estudo/challenge: evita reter snapshot final ao destruir.
  # Em produção, trocar para skip_final_snapshot = false e informar
  # final_snapshot_identifier.
  skip_final_snapshot = true

  tags = {
    Project     = var.cluster_name
    Environment = var.environment
    Terraform   = "true"
  }
}
