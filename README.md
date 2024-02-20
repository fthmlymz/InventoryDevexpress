# Inventory Management .Net Core 7
- https://localhost:2002
- http://localhost:2003


# Architectural Design

Onion Architecture olarak; Domain, Application, Infrasturcture, Persistence ve Presentetion olarak tasarlanmıştır.
MediatR kullanılmıştır.


![alt text](https://github.com/fthmlymz/InventoryManagement/blob/master/assets/ArchitectureLayer.jpg)



# Basic Requirements
**Docker MsSql Server :**
- docker pull mcr.microsoft.com/mssql/server
- docker run -e "ACCEPT_EULA=Y" --name MsSqlServer -e "MSSQL_SA_PASSWORD=Aa123456789*-+" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest


**Docker Serilog(seq) :**
- docker run --name Serilog.Seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest


**Docker RabbitMQ     :**
- docker run -d --hostname RabbitMq --name RabbitMQ -p 5672:5672 -p 15672:15672 rabbitmq:3-management


**Docker Redis        :**
- docker pull redis
- docker run --name Redis -p 6379:6379 -d redis


**Docker Keycloak        :**
- Keycloak klasörü altından docker-compose dosyası kurulmalıdır.

