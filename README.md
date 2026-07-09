# SmartInventory AI - Backend de Negocio

## Descripcion general

Este workspace contiene el backend principal del proyecto **SmartInventory AI**, una aplicacion academica orientada al control de inventario, ventas, facturacion y soporte a compras asistidas por chatbot.

En este repositorio vive la logica de negocio en **.NET**, la persistencia en **PostgreSQL** y la integracion con el servicio de IA que corre en otro workspace. Su responsabilidad es exponer endpoints para el frontend, validar reglas del negocio y registrar operaciones reales como ventas, inventario y facturas.

## Objetivo del proyecto

La idea del proyecto es simular una tienda que desea modernizar su proceso comercial:

- Consultar productos disponibles.
- Revisar existencias reales.
- Registrar ventas manuales o realizadas por chatbot.
- Descontar inventario automaticamente.
- Generar facturas.
- Servir de puente entre el frontend y el microservicio de IA.

## Como se relaciona con los otros workspaces

Este proyecto hace parte de una solucion dividida en tres repositorios:

1. `ProjectNetIa`
   Backend de negocio en ASP.NET Core.
2. `smartinventory-ai-core-main`
   Microservicio de IA en FastAPI, LangChain y LangGraph.
3. `smartinventory-client`
   Frontend en React + TypeScript.

Flujo general:

`React -> .NET -> FastAPI -> .NET -> PostgreSQL`

## Tecnologias principales

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- pgvector
- HttpClient para integracion con el servicio AI

## Estructura del workspace

```text
ProjectNetIa/
|-- Api/               API HTTP, configuracion y controladores
|-- Application/       DTOs e interfaces de aplicacion
|-- Domain/            Entidades del dominio
|-- Infrastructure/    EF Core, servicios, busqueda, migraciones
|-- Scripts/           Scripts de arranque/parada
|-- docker-compose.pgvector.yml
|-- ProjectNetIa.slnx
```

## Explicacion por capas

### `Domain`

Define las entidades principales del negocio, por ejemplo:

- `Product`
- `ProductVariant`
- `Inventory`
- `Sale`
- `Invoice`

Aqui se modela la informacion importante del sistema.

### `Application`

Contiene contratos que conectan controladores con servicios:

- Interfaces
- DTOs de entrada y salida

Esta capa ayuda a mantener separada la logica del negocio del transporte HTTP.

### `Infrastructure`

Implementa la parte tecnica:

- `ApplicationDbContext`
- configuraciones de Entity Framework
- migraciones
- servicios de productos, ventas, inventario y facturas
- integracion con `pgvector`
- generacion y uso de embeddings para busqueda semantica

### `Api`

Expone los endpoints REST que usa el frontend y que tambien consulta el servicio AI.

Algunos endpoints importantes:

- `/api/products`
- `/api/products/variants/search`
- `/api/inventory`
- `/api/sales`
- `/api/invoices`
- `/api/chat/message`

## Funcionalidades importantes del backend

- Gestion de productos y variantes.
- Control de inventario y movimientos.
- Registro de ventas.
- Generacion de facturas.
- Distincion entre ventas manuales y ventas por chatbot.
- Busqueda de productos por texto y por similitud semantica con `pgvector`.

## Busqueda semantica con pgvector

Este proyecto ya incluye integracion de `pgvector` en la base de datos:

- se agrego una columna `Embedding` a `Products`
- se usa la extension `vector` de PostgreSQL
- se creo un indice `hnsw`
- la busqueda de variantes mezcla coincidencia textual con distancia coseno

Esto permite que consultas como:

- `camisa elegante`
- `top negro para salir`
- `tenis blancos comodos`

puedan encontrar productos aunque el usuario no escriba el nombre exacto.

## Base de datos

La opcion recomendada para este proyecto es usar **Docker** con PostgreSQL + `pgvector`.

Archivo incluido:

- [docker-compose.pgvector.yml]

Configuracion actual del backend:

- Host: `localhost`
- Puerto: `5433`
- Base de datos: `ProjectNetIaDb`
- Usuario: `postgres`
- Password: `1234`

## Requisitos para ejecutar

- .NET SDK instalado
- Docker Desktop instalado y en ejecucion
- Python solo si tambien vas a levantar el microservicio AI del otro workspace

## Pasos de ejecucion local

### 1. Levantar PostgreSQL con pgvector

```powershell
docker compose -f docker-compose.pgvector.yml up -d
```

### 2. Aplicar migraciones

```powershell
dotnet ef database update --project Infrastructure --startup-project Api
```

### 3. Ejecutar la API

```powershell
dotnet run --project Api
```

La API queda por defecto en:

- `http://localhost:5083`

## Integracion con el chatbot

El backend no contiene el modelo de IA. En cambio:

- recibe mensajes del frontend
- los reenvia al microservicio FastAPI
- recibe la respuesta estructurada
- mantiene el contrato hacia React

La configuracion del chatbot esta en:

- Api/appsettings.Development.json

## Scripts utiles

En `Scripts/` existen utilidades para ejecutar el stack integrado, especialmente cuando quieras trabajar junto al microservicio AI.

Los nombres importantes son:

- `Start-IntegratedStack.ps1`
- `Start-IntegratedStack-AI.ps1`
- `Stop-IntegratedStack.ps1`


