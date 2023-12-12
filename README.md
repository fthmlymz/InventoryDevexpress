# Inventory Management .Net Core 7
- https://localhost:2002
- http://localhost:2003


# Architectural Design

Onion Architecture olarak; Domain, Application, Infrasturcture, Persistence ve Presentetion olarak tasarlanmıştır.
MediatR kullanılmıştır.


![alt text](https://github.com/fthmlymz/InventoryManagement/blob/master/assets/ArchitectureLayer.jpg)



# Requirements
**Docker Serilog(seq) :**
- docker run --name Serilog.Seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest

**Docker RabbitMQ     :**
- docker run -d --hostname RabbitMq --name RabbitMQ -p 5672:5672 -p 15672:15672 rabbitmq:3-management

**Docker MsSql Server :**
- docker pull mcr.microsoft.com/mssql/server
- docker run -e "ACCEPT_EULA=Y" --name MsSqlServer -e "MSSQL_SA_PASSWORD=Aa123456789*-+" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

**Docker Keycloak     :**
- docker run -p 8080:8080 --name Keycloak -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=Aa123456789*-+ quay.io/keycloak/keycloak:21.1.1 start-dev

**Docker Redis        :**
- docker pull redis
- docker run --name Redis -p 6379:6379 -d redis

