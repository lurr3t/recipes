FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src/usr/app

COPY . .

# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o /out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# install chrome and webdriver
RUN apt update && apt upgrade
RUN apt-get install -y wget
RUN wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
RUN apt-get install -y ./google-chrome-stable_current_amd64.deb

ENTRYPOINT ["dotnet", "dbwbs_projekt.dll"]

EXPOSE 8080