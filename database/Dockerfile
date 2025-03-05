FROM mcr.microsoft.com/azure-sql-edge:latest
# Cant use the standard mssql image on m1 mac
#FROM mcr.microsoft.com/mssql/server:2017-latest

# Create a config directory
RUN mkdir -p /usr/config
WORKDIR /usr/config

# Bundle config source
COPY . /usr/config

# needs to install rust based sqlcmd for m1 mac
RUN mkdir /opt/mssql-tools && mkdir /opt/mssql-tools/bin
COPY sqlcmd /opt/mssql-tools/bin/

# Switch to root user to change permissions
USER root
# Grant permissions for to our scripts to be executable
RUN chmod +x /usr/config/entrypoint.sh
RUN chmod +x /usr/config/configure-db.sh

ENTRYPOINT ["/usr/config/entrypoint.sh"]
