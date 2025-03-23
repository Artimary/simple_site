FROM mcr.microsoft.com/mssql/server:2022-latest

ENV MSSQL_AGENT_ENABLED=true \
    MSSQL_TCP_PORT=1433 \
    MSSQL_LCID=1033 \
    MSSQL_MEMORY_LIMIT_MB=2048

COPY ./scripts/entrypoint.sh /usr/local/bin/
COPY ./scripts/init.sql /docker-entrypoint-initdb.d/

USER root
RUN chmod +x /usr/local/bin/entrypoint.sh && \
    chmod -R 644 /docker-entrypoint-initdb.d/*
USER mssql

ENTRYPOINT ["/usr/local/bin/entrypoint.sh"]