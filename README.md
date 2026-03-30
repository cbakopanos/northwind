# Northwind

## Prerequisites

Install the following before cloning and building.

### OS-level tools

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

## Documentation

- [Domain proposal](docs/DOMAIN.md): DDD proposal with bounded contexts, aggregates, contracts, and context map.
- [Context map diagram](docs/DOMAIN.mmd): Mermaid visual context map.
- [Database reference](docs/DATABASE.md): tables, views, functions, indexes, and constraints.
- [ER diagram](docs/DATABASE.mmd): Mermaid entity-relationship diagram.

## Database setup

- [rundb.sh](database/rundb.sh) (macOS/Linux) / [rundb.ps1](database/rundb.ps1) (Windows): all-in-one script that creates a PostgreSQL 17 Docker container and applies the schema and seed data in one step.
- [init.sql](database/init.sql): schema-partitioned bounded contexts (`sales_ordering`, `catalog`, `crm`, `fulfillment`, `sales_org`, `supplier`, `reporting`) with tables, constraints, views, and functions.
- [seed.sql](database/seed.sql): sample dataset for the physical snake_case schema.
- [seedbmp.sql](database/seedbmp.sql): category picture data (BMP images stored as bytea).
- [instnwnd.sql](database/instnwnd.sql): original Northwind reference script (not used by the setup scripts).

## API setup and inspection

- API project: [Northwind.Api/src/Northwind.Api](Northwind.Api/src/Northwind.Api)
- Core modules project: [Northwind.Api/src/Northwind.Core](Northwind.Api/src/Northwind.Core)
- HTTP request files: [Northwind.Api/src/Northwind.Api/http](Northwind.Api/src/Northwind.Api/http)
- Module loading: attribute + reflection discovery (`ModuleAttribute` + `IModule`) with cached startup scan.

Run API:

1. Start the API:
   - `dotnet run --project Northwind.Api/src/Northwind.Api/Northwind.Api.csproj`
2. Verify health:
   - `GET /api/health`
   - `GET /api/catalog/health`
   - `GET /api/crm/health`
   - `GET /api/fulfillment/health`
   - `GET /api/sales-ordering/health`
   - ~~`GET /api/sales-org/health`~~ — intentionally commented out to demonstrate the alert indicator in the UI
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

## VS Code launch configurations

- **Northwind.Api**: launches the .NET API.
- **Northwind.Web**: starts the Vite dev server and opens the browser.
- **Full Stack**: launches both API and Web together; stopping either stops both.

## CI

- [Build workflow](.github/workflows/build.yml): runs on push/PR to `main`, builds both API (`dotnet build`) and Web (`npm ci && npm run build`) in parallel.

### Run CI locally

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
   - [docs/DOMAIN.md](docs/DOMAIN.md)
8. Open diagram:
    - [docs/DOMAIN.mmd](docs/DOMAIN.mmd)
