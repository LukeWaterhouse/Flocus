FROM postgres:15.1-alpine

ENV PGUSER: postgres
COPY PostgresInit.sh /docker-entrypoint-initdb.d
RUN echo "HI :)"