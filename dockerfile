# Etapa base para o runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Etapa para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Payments.sln", "."]
COPY ["Payments.API/Payments.API.csproj", "Payments.API/"]
COPY ["Payments.Domain/Payments.Domain.csproj", "Payments.Domain/"]
COPY ["Payments.Application/Payments.Application.csproj", "Payments.Application/"]
COPY ["Payments.Data/Payments.Data.csproj", "Payments.Data/"]
COPY ["Payments.Tests/Payments.Tests.csproj", "Payments.Tests/"]

# Restaura as dependências
RUN dotnet restore "Payments.sln"

# Copia o restante do código e realiza o build
COPY . .
WORKDIR "/src/Payments.API"
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Etapa para publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final para execução
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payments.API.dll"]
