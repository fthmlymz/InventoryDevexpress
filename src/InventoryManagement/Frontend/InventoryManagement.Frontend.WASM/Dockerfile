## Stage 1 - Build the Blazor WebAssembly app
#FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
#WORKDIR /src
#COPY ["InventoryManagement.Frontend.csproj", "InventoryManagement/Frontend/"]
#
#RUN dotnet restore "InventoryManagement/Frontend/InventoryManagement.Frontend.csproj"
#COPY . .
#WORKDIR "/src/InventoryManagement/Frontend/InventoryManagement.Frontend"
#
#RUN dotnet build "InventoryManagement.Frontend.csproj" -c Release -o /app/build
#
## Stage 2 - Publish the Blazor WebAssembly app
#FROM build AS publish
#RUN dotnet publish "InventoryManagement.Frontend.csproj" -c Release -o /app/publish
#
## Stage 3 - Create the Nginx container and copy the published app
#FROM nginx:latest AS final
#WORKDIR /usr/share/nginx/html
#COPY --from=publish /app/publish/wwwroot .
#
## Copy the custom nginx configuration if needed
## COPY . /etc/nginx/nginx.conf
#
#EXPOSE 80
#ENTRYPOINT ["nginx", "-g", "daemon off;"]



# Stage 1 - Build the Blazor WebAssembly app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY . .


RUN dotnet restore "InventoryManagement.Frontend.csproj"

#RUN dotnet build -c Release -o /app/build #org
RUN dotnet build InventoryManagement.Frontend.csproj -c Release -o /app/build

# Stage 2 - Publish the Blazor WebAssembly app
FROM build AS publish
#RUN dotnet publish -c Release -o /app/publish #org
RUN dotnet publish InventoryManagement.Frontend.csproj -c Release -o /app/publish

# Stage 3 - Create the Nginx container and copy the published app
FROM nginx:latest AS final
WORKDIR /usr/share/nginx/html

COPY --from=publish /app/publish/wwwroot .

# wwwroot klasörünü kopyala
COPY wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf


COPY ssl/InventoryCert.key /etc/nginx/ssl/InventoryCert.key
COPY ssl/InventoryCert.crt /etc/nginx/ssl/InventoryCert.crt


EXPOSE 4003
EXPOSE 4004

ENTRYPOINT ["nginx", "-g", "daemon off;"]
