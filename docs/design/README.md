# Design documents

> ⚠️ **AI notice**: The contents of this directory are **design and future reference material only**. They do not reflect the current state of the codebase. Do not use them for code generation, consistency checks, or documentation updates unless explicitly instructed.

## Contents

| File | Purpose |
|------|---------|
| `DOMAIN.md` | DDD proposal — bounded contexts, aggregates, contracts, context map (future target; partially aligned in CRM/Catalog/Purchasing) |
| `DOMAIN.mmd` | Mermaid context map diagram (future target reference) |
| `instnwnd.sql` | Original Microsoft Northwind reference SQL script (not used by any setup script) |

## Authoritative sources for current implementation

- [`../DATABASE.md`](../DATABASE.md) — actual database tables, indexes, and constraints
- [`../DATABASE.mmd`](../DATABASE.mmd) — actual ER diagram
- [`../../database/init.sql`](../../database/init.sql) — live schema DDL
- [`../../README.md`](../../README.md) — project overview
