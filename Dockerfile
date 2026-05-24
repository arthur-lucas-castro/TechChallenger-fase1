FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TechChallenger-fase1.sln .
COPY TechChallenger-fase1/TechChallenger-fase1.csproj TechChallenger-fase1/
COPY Repositorios/Repositorios.csproj Repositorios/
COPY Entidades/Entidades.csproj Entidades/
RUN dotnet restore

COPY . .
RUN dotnet publish TechChallenger-fase1/TechChallenger-fase1.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechChallenger-fase1.dll"]
