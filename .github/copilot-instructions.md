# GitHub Copilot Instructions

## Design documents — ignore unless explicitly instructed

The directory `docs/design/` contains **future design and reference material only**. It does not reflect the current state of the codebase.

- `docs/design/DOMAIN.md` — DDD proposal (planned, not implemented)
- `docs/design/DOMAIN.mmd` — context map diagram (planned, not implemented)
- `docs/design/instnwnd.sql` — original Northwind reference SQL (not used by any setup script)

**Do not use any file in `docs/design/` as a source of truth for code generation, consistency checks, or documentation updates unless the user explicitly asks you to.**

The authoritative sources for the current implementation are:
- `docs/DATABASE.md` and `docs/DATABASE.mmd` — actual database state
- `database/init.sql`, `database/seed.sql`, `database/seedbmp.sql` — actual schema and data
- `README.md` — actual project state
