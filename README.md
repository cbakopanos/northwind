# Northwind

## Documentation

- [Domain proposal](docs/DOMAIN.md): DDD proposal with bounded contexts, aggregates, contracts, and context map.
- [Context map diagram](docs/DOMAIN.mmd): Mermaid visual context map.
- [Database reference](docs/DATABASE.md): tables, views, functions, indexes, and constraints.
- [ER diagram](docs/DATABASE.mmd): Mermaid entity-relationship diagram.

## Database setup

- [Container script](database/rundb.sh): creates/starts a local PostgreSQL container only.
- [Schema script](database/initdb.sh): applies [database/init.sql](database/init.sql) to create the complete empty database.
- [Seed script](database/seeddb.sh): optionally loads sample data by executing [database/seed.sql](database/seed.sql) directly.
- [Schema source](database/init.sql): schema-partitioned bounded contexts (`sales_ordering`, `catalog`, `crm`, `fulfillment`, `sales_org`, `supplier`, `reporting`) with tables, constraints, views, and functions.
- [Seed source](database/seed.sql): optional, directly executable sample dataset for the physical snake_case schema.

## API setup and inspection

- API project: [src/Northwind.Api](src/Northwind.Api)
- Core modules project: [src/Northwind.Core](src/Northwind.Core)
- HTTP request files: [src/Northwind.Api/http](src/Northwind.Api/http)
- Module loading: attribute + reflection discovery (`ModuleAttribute` + `IModule`) with cached startup scan.

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
3. Verify supplier reads:
   - `GET /api/supplier/suppliers` (summary list)
   - `GET /api/supplier/suppliers/{supplierId}` (full details)
4. Inspect OpenAPI contract (Development only):
   - `/openapi/v1.json`
   - Note: OpenAPI is mapped only when `ASPNETCORE_ENVIRONMENT=Development`.

### Troubleshooting

- If API startup fails with `address already in use` on `localhost:5019`, stop the process currently using that port:
   - `lsof -nP -iTCP:5019 -sTCP:LISTEN`
   - `kill <PID>`
- Then start the API again.

## Quick start

1. Run:
   - `./database/rundb.sh`
2. Apply schema:
   - `./database/initdb.sh`
3. Optionally load sample data:
   - `./database/seeddb.sh`
4. Start API:
   - `dotnet run --project src/Northwind.Api/Northwind.Api.csproj`
5. Check API contract:
   - `http://localhost:5019/openapi/v1.json`
6. Try requests from:
   - [src/Northwind.Api/http](src/Northwind.Api/http)
7. Read domain docs:
   - [docs/DOMAIN.md](docs/DOMAIN.md)
8. Open diagram:
   - [docs/DOMAIN.mmd](docs/DOMAIN.mmd)
