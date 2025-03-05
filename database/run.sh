source ../.env

docker run --rm -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD' -p 1433:1433 --name sql1 -it mssql-custom