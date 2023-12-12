FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/InventoryManagement/Backend/WebAPI/InventoryManagement.API/InventoryManagement.API.csproj", "src/InventoryManagement/Backend/WebAPI/InventoryManagement.API/"]
COPY ["src/InventoryManagement/Backend/Core/InventoryManagement.Application/InventoryManagement.Application.csproj", "src/InventoryManagement/Backend/Core/InventoryManagement.Application/"]
COPY ["src/InventoryManagement/Shared/SharedLibrary/SharedLibrary.csproj", "src/InventoryManagement/Shared/SharedLibrary/"]
COPY ["src/InventoryManagement/Backend/Core/InventoryManagement.Domain/InventoryManagement.Domain.csproj", "src/InventoryManagement/Backend/Core/InventoryManagement.Domain/"]
COPY ["src/InventoryManagement/Backend/Shared/InventoryManagement.Shared/InventoryManagement.Shared.csproj", "src/InventoryManagement/Backend/Shared/InventoryManagement.Shared/"]
COPY ["src/InventoryManagement/Backend/Infrastructure/InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj", "src/InventoryManagement/Backend/Infrastructure/InventoryManagement.Infrastructure/"]
COPY ["src/InventoryManagement/Backend/Infrastructure/InventoryManagement.Persistence/InventoryManagement.Persistence.csproj", "src/InventoryManagement/Backend/Infrastructure/InventoryManagement.Persistence/"]
RUN dotnet restore "./src/InventoryManagement/Backend/WebAPI/InventoryManagement.API/./InventoryManagement.API.csproj"
COPY . .
WORKDIR "/src/src/InventoryManagement/Backend/WebAPI/InventoryManagement.API"
RUN dotnet build "./InventoryManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InventoryManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


# SSL sertifikalarini kopyala
COPY ["src/InventoryManagement/Backend/WebAPI/InventoryManagement.API/ssl", "/app/ssl/"]


ENTRYPOINT ["dotnet", "InventoryManagement.API.dll"]
