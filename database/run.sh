source ../.env

docker run --rm -e "ACCEPT_EULA=1" -e "MSSQL_SA_PASSWORD=$MSSQL_SA_PASSWORD" -e "MSSQL_PID=Developer" -e "MSSQL_USER=sa" -p 1433:1433 --name=sql1 -it mssql-custom
