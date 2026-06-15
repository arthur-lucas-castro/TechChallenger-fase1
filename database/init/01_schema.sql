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

CREATE TABLE ItemServico (
    Id                       SERIAL PRIMARY KEY,
    Nome                     VARCHAR(50)    NOT NULL,
    PrecoVenda               DECIMAL(10, 2) NOT NULL,
    TempoEstimadoEmMinutos   INTEGER        NOT NULL
);

CREATE TABLE Funcionario (
    Id   SERIAL PRIMARY KEY,
    Nome VARCHAR(50) NOT NULL
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

CREATE TABLE OrdemServico (
    Id                   SERIAL PRIMARY KEY,
    VeiculoId            INTEGER     NOT NULL REFERENCES Veiculo(Id),
    ClienteId            INTEGER     NOT NULL REFERENCES Cliente(Id),
    ResponsavelId        INTEGER     NOT NULL REFERENCES Funcionario(Id),
    Status               VARCHAR(10) NOT NULL,
    DataUltimaAlteracao  TIMESTAMP,
    DataCriacao          TIMESTAMP   NOT NULL,
    DataFinalizacao      TIMESTAMP
);

CREATE TABLE OrdemServicoItem (
    Id               SERIAL PRIMARY KEY,
    OrdemServicoId   INTEGER        NOT NULL REFERENCES OrdemServico(Id),
    ItemServicoId    INTEGER        NOT NULL REFERENCES ItemServico(Id),
    Quantidade       INTEGER        NOT NULL,
    Preco            DECIMAL(10, 2) NOT NULL
);

CREATE TABLE OrdemServicoInsumo (
    Id               SERIAL PRIMARY KEY,
    OrdemServicoId   INTEGER NOT NULL REFERENCES OrdemServico(Id),
    PecaId           INTEGER NOT NULL REFERENCES Peca(Id),
    Quantidade       INTEGER NOT NULL
);

CREATE TABLE Orcamento (
    Id               SERIAL PRIMARY KEY,
    OrdemServicoId   INTEGER        NOT NULL REFERENCES OrdemServico(Id),
    VendedorId       INTEGER        NOT NULL REFERENCES Funcionario(Id),
    PrecoTotal       DECIMAL(10, 2) NOT NULL,
    Status           VARCHAR(10)    NOT NULL,
    DataCriacao      TIMESTAMP      NOT NULL,
    DataEnvio        TIMESTAMP,
    DataAprovacao    TIMESTAMP
);
