FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Inventory/Backend/WebAPI/IM.API/IM.API.csproj", "src/Inventory/Backend/WebAPI/IM.API/"]
COPY ["src/Inventory/Backend/Core/IM.Application/IM.Application.csproj", "src/Inventory/Backend/Core/IM.Application/"]
COPY ["src/Inventory/Shared/SharedLibrary/SharedLibrary.csproj", "src/Inventory/Shared/SharedLibrary/"]
COPY ["src/Inventory/Backend/Core/IM.Domain/IM.Domain.csproj", "src/Inventory/Backend/Core/IM.Domain/"]
COPY ["src/Inventory/Backend/Shared/IM.Shared/IM.Shared.csproj", "src/Inventory/Backend/Shared/IM.Shared/"]
COPY ["src/Inventory/Backend/Infrastructure/IM.Infrastructure/IM.Infrastructure.csproj", "src/Inventory/Backend/Infrastructure/IM.Infrastructure/"]
COPY ["src/Inventory/Backend/Infrastructure/IM.Persistence/IM.Persistence.csproj", "src/Inventory/Backend/Infrastructure/IM.Persistence/"]
RUN dotnet restore "./src/Inventory/Backend/WebAPI/IM.API/./IM.API.csproj"
COPY . .
WORKDIR "/src/src/Inventory/Backend/WebAPI/IM.API"
RUN dotnet build "./IM.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IM.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# SSL sertifikalarini kopyala
COPY ["src/Inventory/Backend/WebAPI/IM.API/ssl", "/app/ssl/"]


ENTRYPOINT ["dotnet", "IM.API.dll"]
