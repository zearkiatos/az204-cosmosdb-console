FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

RUN apt-get update && \
    apt-get upgrade -y

COPY . /app

CMD dotnet build az204-cosmosdb-console.sln && \
    dotnet run