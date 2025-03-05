#!/bin/bash

# Wait 60 seconds for SQL Server to start up by ensuring that 
# calling SQLCMD does not return an error code, which will ensure that sqlcmd is accessible
# and that system and user databases return "0" which means all databases are in an "online" state
# https://docs.microsoft.com/en-us/sql/relational-databases/system-catalog-views/sys-databases-transact-sql?view=sql-server-2017 


# Wait for SQL Server to start
DBSTATUS=1
ERRCODE=1
while [ $DBSTATUS -ne 0 ] || [ $ERRCODE -ne 0 ]; do
    sleep 1
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -Q "SELECT 1" &> /dev/null
    DBSTATUS=$?
    ERRCODE=$?
done

if [ $DBSTATUS -ne 0 ] || [ $ERRCODE -ne 0 ]; then 
    echo "SQL Server took more than 60 seconds to start up or one or more databases are not in an ONLINE state"
    exit 1
fi

# Retry mechanism for database creation
RETRY_COUNT=5
RETRY_INTERVAL=5
for i in $(seq 1 $RETRY_COUNT); do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -Q "CREATE DATABASE projekt" && break
    echo "Retrying database creation in $RETRY_INTERVAL seconds..."
    sleep $RETRY_INTERVAL
done

# Run the setup script to create the schema in the DB
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -d projekt -i sql_scripts/create.sql