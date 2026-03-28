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

## Quick start

1. Run:
   - `./database/rundb.sh`
2. Read domain docs:
   - [docs/DOMAIN.md](docs/DOMAIN.md)
3. Open diagram:
   - [docs/DOMAIN.mmd](docs/DOMAIN.mmd)
