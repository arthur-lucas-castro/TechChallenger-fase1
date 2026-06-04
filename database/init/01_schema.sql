CREATE TYPE tipo_pessoa AS ENUM ('F', 'J');

CREATE TABLE cliente (
    id               SERIAL PRIMARY KEY,
    nome             VARCHAR(50)  NOT NULL,
    sobrenome        VARCHAR(50)  NOT NULL,
    telefone         VARCHAR(11)  NOT NULL,
    email            VARCHAR(50)  NOT NULL,
    numero_documento VARCHAR(14)  NOT NULL,
    tipo_pessoa      tipo_pessoa  NOT NULL
);

CREATE TABLE veiculo (
    id         SERIAL PRIMARY KEY,
    modelo     VARCHAR(50) NOT NULL,
    placa      VARCHAR(7)  NOT NULL,
    marca      VARCHAR(50) NOT NULL,
    ano        INTEGER     NOT NULL,
    cliente_id INTEGER     NOT NULL REFERENCES cliente(id)
);

CREATE TABLE tipo_servico (
    id                        SERIAL PRIMARY KEY,
    nome                      VARCHAR(50)    NOT NULL,
    preco_venda               DECIMAL(10, 2) NOT NULL,
    tempo_estimado_em_minutos INTEGER        NOT NULL
);

CREATE TABLE funcionario (
    id   SERIAL PRIMARY KEY,
    nome VARCHAR(50) NOT NULL
);

CREATE TABLE insumo (
    id          SERIAL PRIMARY KEY,
    nome        VARCHAR(50)    NOT NULL,
    descricao   VARCHAR(50)    NOT NULL,
    custo       DECIMAL(10, 2) NOT NULL,
    preco_venda DECIMAL(10, 2) NOT NULL
);

CREATE TABLE ordem_servico (
    id                    SERIAL PRIMARY KEY,
    veiculo_id            INTEGER     NOT NULL REFERENCES veiculo(id),
    responsavel_id        INTEGER     NOT NULL REFERENCES funcionario(id),
    status                VARCHAR(10) NOT NULL,
    data_ultima_alteracao TIMESTAMP,
    data_criacao          TIMESTAMP   NOT NULL,
    data_finalizacao      TIMESTAMP
);

CREATE TABLE ordem_servico_item (
    id               SERIAL PRIMARY KEY,
    ordem_servico_id INTEGER        NOT NULL REFERENCES ordem_servico(id),
    tipo_servico_id  INTEGER        NOT NULL REFERENCES tipo_servico(id),
    quantidade       INTEGER        NOT NULL,
    preco            DECIMAL(10, 2) NOT NULL
);

CREATE TABLE ordem_servico_insumo (
    id               SERIAL PRIMARY KEY,
    ordem_servico_id INTEGER NOT NULL REFERENCES ordem_servico(id),
    insumo_id        INTEGER NOT NULL REFERENCES insumo(id),
    quantidade       INTEGER NOT NULL
);

CREATE TABLE orcamento (
    id               SERIAL PRIMARY KEY,
    ordem_servico_id INTEGER        NOT NULL REFERENCES ordem_servico(id),
    vendedor_id      INTEGER        NOT NULL REFERENCES funcionario(id),
    preco_total      DECIMAL(10, 2) NOT NULL,
    status           VARCHAR(10)    NOT NULL,
    data_criacao     TIMESTAMP      NOT NULL,
    data_envio       TIMESTAMP,
    data_aprovacao   TIMESTAMP
);
