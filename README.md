# Northwind

<p align="center">
  <img alt=".NET" src="https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white">
  <img alt="C#" src="https://img.shields.io/badge/C%23-13-239120?logo=csharp&logoColor=white">
  <img alt="React" src="https://img.shields.io/badge/React-19-61DAFB?logo=react&logoColor=black">
  <img alt="TypeScript" src="https://img.shields.io/badge/TypeScript-5-3178C6?logo=typescript&logoColor=white">
  <img alt="Vite" src="https://img.shields.io/badge/Vite-6-646CFF?logo=vite&logoColor=white">
  <img alt="PostgreSQL" src="https://img.shields.io/badge/PostgreSQL-17-4169E1?logo=postgresql&logoColor=white">
  <img alt="Docker" src="https://img.shields.io/badge/Docker-required-2496ED?logo=docker&logoColor=white">
  <img alt="CI" src="https://img.shields.io/github/actions/workflow/status/cbakopanos/northwind/build.yml?label=CI&logo=githubactions&logoColor=white">
  <img alt="License" src="https://img.shields.io/badge/license-MIT-green">
</p>

--- 

> **Northwind** is a full-stack reference application — ASP.NET Core Minimal API backend with a Vite + React + TypeScript frontend, backed by PostgreSQL 17, structured around DDD bounded contexts.

---

## Prerequisites

Install the following before cloning and building.

### OS-level tools

<details>
<summary>Installation commands per OS</summary>

| Tool | macOS | Windows |
|------|-------|---------|
| **Git** | `brew install git` | [git-scm.com](https://git-scm.com/) |
| **.NET 10 SDK** | `brew install dotnet@10` | [dot.net/download](https://dot.net/download) |
| **Node.js 22+** | `brew install node` | [nodejs.org](https://nodejs.org/) (LTS) |
| **Docker Desktop** | [docker.com](https://www.docker.com/products/docker-desktop/) | [docker.com](https://www.docker.com/products/docker-desktop/) |
| **act** *(optional)* | `brew install act` | [nektosact.com](https://nektosact.com/) |

> Docker is only needed for the PostgreSQL database container.
> [`act`](https://github.com/nektos/act) lets you run GitHub Actions workflows locally (requires Docker).

### VS Code extensions

Install [Visual Studio Code](https://code.visualstudio.com/), then add these extensions:

| Extension | ID |
|-----------|----|
| C# Dev Kit | `ms-dotnettools.csdevkit` |
| REST Client | `humao.rest-client` |
| Mermaid Preview | `vstirbu.vscode-mermaid-preview` |
| PostgreSQL | `ms-ossdata.vscode-pgsql` |

### Verify your setup

```bash
git --version            # any recent version
dotnet --version         # 10.x
node --version           # 22.x or later
npm --version            # 10.x or later
docker --version         # any recent version
```

</details>

---

## Documentation

- [Domain proposal](docs/design/DOMAIN.md): future-state DDD proposal (reference only, not source of truth for current implementation).
- [Context map diagram](docs/design/DOMAIN.mmd): future-state Mermaid context map (reference only).
- [Database reference](docs/DATABASE.md): tables, views, functions, indexes, and constraints.
- [ER diagram](docs/DATABASE.mmd): Mermaid entity-relationship diagram.
- [Code review](docs/REVIEW.md): SOLID, DDD, maintainability, scalability, modularity and performance analysis.

### Current implementation status (as-is)

- Implemented schemas/tables: `purchasing.suppliers`, `catalog.categories`, `catalog.products`, `crm.customers`.
- DDD behavior-centric implementation:
   - CRM: `Customer` aggregate + domain repository + application service.
   - Catalog: `Product` aggregate + domain repository + application service.
   - Purchasing: `Supplier` aggregate + domain repository + application service.
- Intentional mixed mode in Catalog:
   - `Product` follows DDD flow.
   - `Category` remains straightforward CRUD.
- Domain-validation exception mapping is centralized with endpoint filters for CRM, Catalog (Product routes), and Purchasing.

### Code Review Ratings

| Category | Backend | Frontend | Overall |
|---|:---:|:---:|:---:|
| SOLID | 8 / 10 | 7 / 10 | **8 / 10** |
| DDD | 7 / 10 | 5 / 10 | **7 / 10** |
| Clean Architecture | 6 / 10 | 6 / 10 | **6 / 10** |
| Maintainability | 7 / 10 | 7 / 10 | **7 / 10** |
| Scalability | 6 / 10 | 6 / 10 | **6 / 10** |
| Modularity | 8 / 10 | 7 / 10 | **8 / 10** |
| Performance | 7 / 10 | 6 / 10 | **7 / 10** |
| **Overall** | **7 / 10** | **6.5 / 10** | **7 / 10** |

---

## Database setup

- [rundb.sh](database/rundb.sh) (macOS/Linux) / [rundb.ps1](database/rundb.ps1) (Windows): all-in-one script that creates a PostgreSQL 17 Docker container and applies the schema and seed data in one step.
- [init.sql](database/init.sql): schema-partitioned bounded contexts (`sales_ordering`, `catalog`, `crm`, `fulfillment`, `sales_org`, `purchasing`, `reporting`) with tables, constraints, views, and functions.
- [seed.sql](database/seed.sql): sample dataset for the physical snake_case schema.
- [seedbmp.sql](database/seedbmp.sql): category picture data (BMP images stored as bytea).
- [instnwnd.sql](docs/design/instnwnd.sql): original Northwind reference script (design reference only, not used by the setup scripts).

---

## API setup and inspection

- API project: [Northwind.Api/src/Northwind.Api](Northwind.Api/src/Northwind.Api)
- Core modules project: [Northwind.Api/src/Northwind.Core](Northwind.Api/src/Northwind.Core)
- HTTP request files: [Northwind.Api/src/Northwind.Api/http](Northwind.Api/src/Northwind.Api/http)
- Module loading: reflection-based discovery — all `IModule` implementations are scanned at startup, ordered alphabetically, and registered automatically.

Run API:

1. Start the API:
   - `dotnet run --project Northwind.Api/src/Northwind.Api/Northwind.Api.csproj`
2. Verify health:
   - `GET /api/health`
   - `GET /api/catalog/health`
   - `GET /api/crm/health`
   - `GET /api/fulfillment/health`
   - `GET /api/sales-ordering/health`
   - `GET /api/sales-org/health`
   - `GET /api/purchasing/health`
   - `GET /api/reporting/health`
3. Verify purchasing endpoints:
   - `GET /api/purchasing/suppliers` (summary list, supports `page` and `pageSize` query params)
   - `GET /api/purchasing/suppliers/{supplierId}` (full details)
   - `POST /api/purchasing/suppliers` (create supplier)
   - `PUT /api/purchasing/suppliers/{supplierId}` (update supplier)
4. Inspect OpenAPI contract (Development only):
   - `/openapi/v1.json`
   - Note: OpenAPI is mapped only when `ASPNETCORE_ENVIRONMENT=Development`.

<details>
<summary>Troubleshooting</summary>

- If API startup fails with `address already in use` on `localhost:5019`, stop the process currently using that port:
   - `lsof -nP -iTCP:5019 -sTCP:LISTEN`
   - `kill <PID>`
- Then start the API again.

</details>

---

## Web UI

- Web project: [Northwind.Web](Northwind.Web)
- Stack: Vite + React + TypeScript, Tailwind CSS, React Router, TanStack Query.
- Feature modules: each bounded context has its own folder under `src/features/` with a barrel `index.ts`.

Run Web:

1. Install dependencies (first time only):
   - `cd Northwind.Web && npm ci`
2. Start dev server:
   - `npm run dev`
3. Open browser:
   - `http://localhost:3000`

The Vite dev server proxies `/api/*` requests to `http://localhost:5019`.

---

## VS Code launch configurations

- **Northwind.Api**: launches the .NET API.
- **Northwind.Web**: starts the Vite dev server and opens the browser.
- **Full Stack**: launches both API and Web together; stopping either stops both.

---

## CI

- [Build workflow](.github/workflows/build.yml): runs on push/PR to `main`, builds both API (`dotnet build`) and Web (`npm ci && npm run build`) in parallel.

<details>
<summary>Run CI locally</summary>

Install [`act`](https://github.com/nektos/act) to run GitHub Actions workflows locally (requires Docker):

```bash
brew install act
act            # run all jobs
act -j api     # run just the API build
act -j web     # run just the Web build
```

Alternatively, use the VS Code build tasks (no Docker required):

- **Build API**: `dotnet build --configuration Release` in `Northwind.Api/`
- **Build Web**: `npm ci && npm run build` in `Northwind.Web/`
- **Build All**: runs both in parallel (default build task — `Ctrl+Shift+B`)

</details>

---

## Quick start

1. Create and seed the database:
   - macOS/Linux: `./database/rundb.sh`
   - Windows: `.\database\rundb.ps1`
2. Start API:
   - `dotnet run --project Northwind.Api/src/Northwind.Api/Northwind.Api.csproj`
3. Start Web:
   - `cd Northwind.Web && npm ci && npm run dev`
4. Open browser:
   - `http://localhost:3000`
5. Check API contract:
   - `http://localhost:5019/openapi/v1.json`
6. Try requests from:
   - [Northwind.Api/src/Northwind.Api/http](Northwind.Api/src/Northwind.Api/http)
7. Read domain docs:
   - [docs/design/DOMAIN.md](docs/design/DOMAIN.md)
8. Open diagram:
    - [docs/design/DOMAIN.mmd](docs/design/DOMAIN.mmd)
