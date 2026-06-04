FROM postgres:16-alpine

ENV POSTGRES_DB=techchallenger
ENV POSTGRES_USER=postgres
ENV POSTGRES_PASSWORD=postgres

COPY init/ /docker-entrypoint-initdb.d/

EXPOSE 5432
