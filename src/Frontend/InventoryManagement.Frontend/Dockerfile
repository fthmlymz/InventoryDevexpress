FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Inventory/Frontend/InventoryManagement.Frontend/InventoryManagement.Frontend.csproj", "src/Inventory/Frontend/InventoryManagement.Frontend/"]
COPY ["src/Inventory/Backend/Core/IM.Domain/IM.Domain.csproj", "src/Inventory/Backend/Core/IM.Domain/"]
COPY ["src/Inventory/Shared/SharedLibrary/SharedLibrary.csproj", "src/Inventory/Shared/SharedLibrary/"]

# copy local nuget packages - start
COPY "src/Inventory/Frontend/InventoryManagement.Frontend/nugets/" "/var/nugets/"
RUN dotnet nuget add source /var/nugets/
# copy local nuget packages - end


# Copy Report Files - start
COPY "src/Inventory/Frontend/InventoryManagement.Frontend/Reports/" "app/Reports/"
# Copy Report Files - end


RUN dotnet restore "./src/Inventory/Frontend/InventoryManagement.Frontend/./InventoryManagement.Frontend.csproj"

COPY . .
WORKDIR "/src/src/Inventory/Frontend/InventoryManagement.Frontend"
RUN dotnet build "./InventoryManagement.Frontend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InventoryManagement.Frontend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


# sil cert
#RUN apt-get update
#RUN apt-get install ca-certificates
# sil cert



# devexress skiasharp - install
RUN apt-get update
RUN apt-get install -y libc6 libgdiplus libicu-dev libharfbuzz0b libfontconfig1 libfreetype6
RUN apt-get install -y libpango-1.0-0 libpangocairo-1.0
# devexress skiasharp - install end

# SSL sertifikalarini kopyala
#COPY ["src/Inventory/Frontend/InventoryManagement.Frontend/ssl", "/app/ssl/"]
# host.docker.internal - ssl start
COPY ["/src/Inventory/Frontend/InventoryManagement.Frontend/ssl/aspnetapp.pfx", "/app/ssl/tls.pfx"]
COPY ["/src/Inventory/Frontend/InventoryManagement.Frontend/ssl/docker-host.crt", "/etc/x509/https/docker-host.crt"]
COPY ["/src/Inventory/Frontend/InventoryManagement.Frontend/ssl/docker-host.key", "/etc/x509/https/docker-host.key"]
#COPY ["src/Inventory/Frontend/InventoryManagement.Frontend/ssl", "/etc/x509/https/"]
# host.docker.internal - ssl end

ENTRYPOINT ["dotnet", "InventoryManagement.Frontend.dll"]
