# Northwind

## Documentation

- [Domain proposal](docs/DOMAIN.md): DDD proposal with bounded contexts, aggregates, contracts, and context map.
- [Context map diagram](docs/MAP.mmd): Mermaid visual context map.

## Database setup

- [Bootstrap script](database/rundb.sh): starts a local PostgreSQL container and loads schema + seed.
- [Schema](database/init.sql): tables, constraints, views, and functions.
- [Seed data](database/seed.sql): dataset load script.

## Quick start

1. Run:
   - `./database/rundb.sh`
2. Read domain docs:
   - [docs/DOMAIN.md](docs/DOMAIN.md)
3. Open diagram:
   - [docs/MAP.mmd](docs/MAP.mmd)
