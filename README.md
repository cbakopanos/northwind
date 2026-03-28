# Northwind

## Documentation

- [Domain proposal](docs/DOMAIN.md): DDD proposal with bounded contexts, aggregates, contracts, and context map.
- [Context map diagram](docs/DOMAIN.mmd): Mermaid visual context map.
- [Database reference](docs/DATABASE.md): tables, views, functions, indexes, and constraints.
- [ER diagram](docs/DATABASE.mmd): Mermaid entity-relationship diagram.

## Database setup

- [Bootstrap script](database/rundb.sh): starts a local PostgreSQL container and loads schema + seed.
- [Schema](database/init.sql): schema-partitioned bounded contexts (`sales_ordering`, `catalog`, `crm`, `fulfillment`, `sales_org`, `supplier`, `reporting`) with tables, constraints, views, and functions.
- [Seed data](database/seed.sql): legacy-form dataset load script (translated to physical snake_case table/column names by bootstrap preprocessing).

## API setup and inspection

- API project: [src/Northwind.Api](src/Northwind.Api)
- HTTP request files: [src/Northwind.Api/http](src/Northwind.Api/http)

Run API:

1. Start the API:
   - `dotnet run --project src/Northwind.Api/Northwind.Api.csproj`
2. Verify health:
   - `GET /api/health`
   - `GET /api/catalog/health`
   - `GET /api/crm/health`
   - `GET /api/fulfillment/health`
   - `GET /api/sales-ordering/health`
   - `GET /api/sales-org/health`
   - `GET /api/supplier/health`
   - `GET /api/reporting/health`
3. Inspect OpenAPI contract (Development only):
   - `/openapi/v1.json`

## Quick start

1. Run:
   - `./database/rundb.sh`
2. Start API:
   - `dotnet run --project src/Northwind.Api/Northwind.Api.csproj`
3. Check API contract:
   - `http://localhost:5019/openapi/v1.json`
4. Try requests from:
   - [src/Northwind.Api/http](src/Northwind.Api/http)
5. Read domain docs:
   - [docs/DOMAIN.md](docs/DOMAIN.md)
6. Open diagram:
   - [docs/DOMAIN.mmd](docs/DOMAIN.mmd)
