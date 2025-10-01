FROM postgres:16-alpine
# ENV POSTGRES_USER=devuser \
#     POSTGRES_PASSWORD=secret123 \
#     POSTGRES_DB=devdb
COPY ./init.sql /docker-entrypoint-initdb.d/
EXPOSE 5432
VOLUME /var/lib/postgresql/data