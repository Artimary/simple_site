#!/bin/sh
/opt/mssql/bin/sqlservr &
sleep 30
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P StrongP@ssw0rd -C -i /docker-entrypoint-initdb.d/init.sql
wait