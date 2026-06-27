CREATE TABLE Cliente (
    Id               SERIAL PRIMARY KEY,
    Nome             VARCHAR(50)  NOT NULL,
    Sobrenome        VARCHAR(50)  NOT NULL,
    Telefone         VARCHAR(11)  NOT NULL,
    Email            VARCHAR(50)  NOT NULL,
    NumeroDocumento  VARCHAR(14)  NOT NULL,
    TipoPessoa       VARCHAR(1)   NOT NULL
);

CREATE TABLE Veiculo (
    Id      SERIAL PRIMARY KEY,
    Modelo  VARCHAR(50) NOT NULL,
    Placa   VARCHAR(7)  NOT NULL UNIQUE,
    Marca   VARCHAR(50) NOT NULL,
    Ano     INTEGER     NOT NULL
);

CREATE TABLE ClienteVeiculo (
    ClienteId  INTEGER NOT NULL REFERENCES Cliente(Id),
    VeiculoId  INTEGER NOT NULL REFERENCES Veiculo(Id),
    PRIMARY KEY (ClienteId, VeiculoId)
);

CREATE TABLE Servico (
    Id                       SERIAL PRIMARY KEY,
    Nome                     VARCHAR(50)    NOT NULL,
    PrecoVenda               DECIMAL(10, 2) NOT NULL,
    TempoEstimadoEmMinutos   INTEGER        NOT NULL
);

CREATE TABLE Peca (
    Id          SERIAL PRIMARY KEY,
    Nome        VARCHAR(50)    NOT NULL,
    Descricao   VARCHAR(50)    NOT NULL,
    Custo       DECIMAL(10, 2) NOT NULL,
    PrecoVenda  DECIMAL(10, 2) NOT NULL
);

CREATE TABLE ProdutoEstoque (
    Id               SERIAL PRIMARY KEY,
    PecaId           INTEGER        NOT NULL REFERENCES Peca(Id),
    QuantidadeAtual  INTEGER        NOT NULL DEFAULT 0,
    QuantidadeMinima INTEGER        NOT NULL,
    PrecoCustoMedio  DECIMAL(10, 2) NOT NULL
);

CREATE TYPE status_ordem_servico AS ENUM (
    'recebida',
    'em_diagnostico',
    'aguardando_aprovacao',
    'em_execucao',
    'finalizada',
    'entregue'
);

CREATE TABLE OrdemServico (
    Id                   SERIAL PRIMARY KEY,
    VeiculoId            INTEGER     NOT NULL REFERENCES Veiculo(Id),
    ClienteId            INTEGER     NOT NULL REFERENCES Cliente(Id),
    Status               status_ordem_servico NOT NULL,
    DataUltimaAlteracao  TIMESTAMP,
    DataCriacao          TIMESTAMP   NOT NULL,
    DataFinalizacao      TIMESTAMP
);

CREATE TABLE ServicoSolicitado (
    Id               SERIAL PRIMARY KEY,
    OrdemServicoId   INTEGER        NOT NULL REFERENCES OrdemServico(Id),
    ServicoId        INTEGER        NOT NULL REFERENCES Servico(Id),
    Quantidade       INTEGER        NOT NULL,
    PrecoVenda       DECIMAL(10, 2) NOT NULL
);


CREATE TABLE PecaSolicitada (
    Id               SERIAL PRIMARY KEY,
    OrdemServicoId   INTEGER        NOT NULL REFERENCES OrdemServico(Id),
    PecaId           INTEGER        NOT NULL REFERENCES Peca(Id),
    Quantidade       INTEGER        NOT NULL,
    Nome             VARCHAR(100)   NOT NULL,
    PrecoVenda       DECIMAL(10, 2) NOT NULL
);

CREATE TABLE ServicoExecucao (
    Id                   SERIAL PRIMARY KEY,
    ServicoSolicitadoId  INTEGER      NOT NULL REFERENCES ServicoSolicitado(Id),
    Status               VARCHAR(20)  NOT NULL,
    DataInicio           TIMESTAMP,
    DataFinalizacao      TIMESTAMP
);

CREATE TABLE Orcamento (
    Id               SERIAL PRIMARY KEY,
    OrdemServicoId   INTEGER        NOT NULL REFERENCES OrdemServico(Id),
    PrecoTotal       DECIMAL(10, 2) NOT NULL,
    Status           VARCHAR(10)    NOT NULL,
    DataCriacao      TIMESTAMP      NOT NULL,
    DataEnvio        TIMESTAMP,
    DataAprovacao    TIMESTAMP
);

CREATE TABLE usuario (
    id         SERIAL PRIMARY KEY,
    email      VARCHAR(100) NOT NULL UNIQUE,
    senhahash  VARCHAR(72)  NOT NULL,
    tipo       VARCHAR(20)  NOT NULL
);

INSERT INTO usuario (email, senhahash, tipo) VALUES
    ('adm@oficina.com',         '$2a$12$.r/874bZqjmBZNPkYf7el.6c9e6apb39cOUypaELCiKPw78.BSNIK', 'Adm'),
    ('funcionario@oficina.com', '$2a$12$Rs8rAOD6Edl9DDwliF15f.k2HnHGSjEZGJ8RYLce69cEVNNWI4bwe', 'Funcionario');
