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
COPY /Models/GptHandler/prePromptGet.txt .
COPY /Models/GptHandler/prePromptJoin.txt .


# install chrome and webdriver
#RUN apt-get update && apt-get install -y wget gnupg2
#RUN wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add -
#RUN sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list'
#RUN apt-get update && apt-get install -y google-chrome-stable

ENTRYPOINT ["dotnet", "dbwbs_projekt.dll"]

EXPOSE 8080