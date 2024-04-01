#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Inventory/src/Backend/WebAPI/Web.API/Web.API.csproj", "Inventory/src/Backend/WebAPI/Web.API/"]
COPY ["Inventory/src/Backend/Core/Application/Application.csproj", "Inventory/src/Backend/Core/Application/"]
COPY ["Inventory/src/Shared/SharedLibrary/SharedLibrary.csproj", "Inventory/src/Shared/SharedLibrary/"]
COPY ["Inventory/src/Backend/Core/Domain/Domain.csproj", "Inventory/src/Backend/Core/Domain/"]
COPY ["Inventory/src/Backend/Shared/Shared/Shared.csproj", "Inventory/src/Backend/Shared/Shared/"]
COPY ["Inventory/src/Backend/Infrastructure/Infrastructure/Infrastructure.csproj", "Inventory/src/Backend/Infrastructure/Infrastructure/"]
COPY ["Inventory/src/Backend/Infrastructure/Persistence/Persistence.csproj", "Inventory/src/Backend/Infrastructure/Persistence/"]
RUN dotnet restore "./Inventory/src/Backend/WebAPI/Web.API/Web.API.csproj"
COPY . .
WORKDIR "/src/Inventory/src/Backend/WebAPI/Web.API"
RUN dotnet build "./Web.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


# SSL sertifikalarini kopyala
COPY ["src/Inventory/Backend/WebAPI/Web.API/ssl", "/app/ssl/"]


ENTRYPOINT ["dotnet", "Web.API.dll"]