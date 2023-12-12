FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/InventoryManagement/Gateway/ApiManagement.Gateway/ApiManagement.Gateway.csproj", "src/InventoryManagement/Gateway/ApiManagement.Gateway/"]
RUN dotnet restore "./src/InventoryManagement/Gateway/ApiManagement.Gateway/./ApiManagement.Gateway.csproj"
COPY . .
WORKDIR "/src/src/InventoryManagement/Gateway/ApiManagement.Gateway"
RUN dotnet build "./ApiManagement.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApiManagement.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


# host.docker.internal
COPY ["src/InventoryManagement/Gateway/ApiManagement.Gateway/ssl/aspnetapp.pfx", "/app/ssl/tls.pfx"]

COPY ["src/InventoryManagement/Gateway/ApiManagement.Gateway/ssl/docker-host.crt", "/etc/x509/https/docker-host.crt"]
COPY ["src/InventoryManagement/Gateway/ApiManagement.Gateway/ssl/docker-host.key", "/etc/x509/https/docker-host.key"]
# host.docker.internal


ENTRYPOINT ["dotnet", "ApiManagement.Gateway.dll"]