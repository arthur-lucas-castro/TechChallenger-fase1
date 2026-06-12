CREATE TABLE Clientes (
    Id               SERIAL PRIMARY KEY,
    Nome             VARCHAR(50)  NOT NULL,
    Sobrenome        VARCHAR(50)  NOT NULL,
    Telefone         VARCHAR(11)  NOT NULL,
    Email            VARCHAR(50)  NOT NULL,
    NumeroDocumento  VARCHAR(14)  NOT NULL,
    TipoPessoa       VARCHAR(1)   NOT NULL
);

CREATE TABLE Veiculos (
    Id      SERIAL PRIMARY KEY,
    Modelo  VARCHAR(50) NOT NULL,
    Placa   VARCHAR(7)  NOT NULL UNIQUE,
    Marca   VARCHAR(50) NOT NULL,
    Ano     INTEGER     NOT NULL
);

CREATE TABLE ClienteVeiculos (
    ClienteId  INTEGER NOT NULL REFERENCES Clientes(Id),
    VeiculoId  INTEGER NOT NULL REFERENCES Veiculos(Id),
    PRIMARY KEY (ClienteId, VeiculoId)
);

CREATE TABLE ItemServicos (
    Id                       SERIAL PRIMARY KEY,
    Nome                     VARCHAR(50)    NOT NULL,
    PrecoVenda               DECIMAL(10, 2) NOT NULL,
    TempoEstimadoEmMinutos   INTEGER        NOT NULL
);

CREATE TABLE Funcionarios (
    Id   SERIAL PRIMARY KEY,
    Nome VARCHAR(50) NOT NULL
);

CREATE TABLE Pecas (
    Id          SERIAL PRIMARY KEY,
    Nome        VARCHAR(50)    NOT NULL,
    Descricao   VARCHAR(50)    NOT NULL,
    Custo       DECIMAL(10, 2) NOT NULL,
    PrecoVenda  DECIMAL(10, 2) NOT NULL
);

CREATE TABLE OrdemServicos (
    Id                   SERIAL PRIMARY KEY,
    VeiculoId            INTEGER     NOT NULL REFERENCES Veiculos(Id),
    ClienteId            INTEGER     NOT NULL REFERENCES Clientes(Id),
    ResponsavelId        INTEGER     NOT NULL REFERENCES Funcionarios(Id),
    Status               VARCHAR(10) NOT NULL,
    DataUltimaAlteracao  TIMESTAMP,
    DataCriacao          TIMESTAMP   NOT NULL,
    DataFinalizacao      TIMESTAMP
);

CREATE TABLE OrdemServicoItems (
    Id               SERIAL PRIMARY KEY,
    OrdemServicoId   INTEGER        NOT NULL REFERENCES OrdemServicos(Id),
    ItemServicoId    INTEGER        NOT NULL REFERENCES ItemServicos(Id),
    Quantidade       INTEGER        NOT NULL,
    Preco            DECIMAL(10, 2) NOT NULL
);

CREATE TABLE OrdemServicoInsumos (
    Id               SERIAL PRIMARY KEY,
    OrdemServicoId   INTEGER NOT NULL REFERENCES OrdemServicos(Id),
    PecaId           INTEGER NOT NULL REFERENCES Pecas(Id),
    Quantidade       INTEGER NOT NULL
);

CREATE TABLE Orcamentos (
    Id               SERIAL PRIMARY KEY,
    OrdemServicoId   INTEGER        NOT NULL REFERENCES OrdemServicos(Id),
    VendedorId       INTEGER        NOT NULL REFERENCES Funcionarios(Id),
    PrecoTotal       DECIMAL(10, 2) NOT NULL,
    Status           VARCHAR(10)    NOT NULL,
    DataCriacao      TIMESTAMP      NOT NULL,
    DataEnvio        TIMESTAMP,
    DataAprovacao    TIMESTAMP
);
