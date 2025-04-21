# Etapa base para o runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Etapa para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Core.API.sln", "."]
COPY ["Core.API/Core.API.csproj", "Core.API/"]
COPY ["Core.Domain/Core.Domain.csproj", "Core.Domain/"]
COPY ["Core.Service/Core.Service.csproj", "Core.Service/"]
COPY ["Core.Data/Core.Data.csproj", "Core.Data/"]
COPY ["Core.Tests/Core.Tests.csproj", "Core.Tests/"]

# Restaura as dependências
RUN dotnet restore "Core.API.sln"

# Copia o restante do código e realiza o build
COPY . .
WORKDIR "/src/Core.API"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Etapa para publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final para execução
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Core.API.dll"]
