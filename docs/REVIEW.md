# Code Review

> **Scope:** SOLID principles · DDD alignment · Maintainability · Scalability · Modularity · Performance  
> **Date:** April 2026  
> **Stack:** .NET 10 Minimal API · EF Core 10 · PostgreSQL 17 · React 19 · TypeScript · Vite · TanStack Query

---

## Ratings

| Category | Backend | Frontend | Overall |
|---|:---:|:---:|:---:|
| **SOLID** | 8 / 10 | 7 / 10 | **8 / 10** |
| **DDD** | 6 / 10 | 5 / 10 | **6 / 10** |
| **Clean Architecture** | 6 / 10 | 6 / 10 | **6 / 10** |
| **Maintainability** | 7 / 10 | 7 / 10 | **7 / 10** |
| **Scalability** | 6 / 10 | 6 / 10 | **6 / 10** |
| **Modularity** | 8 / 10 | 7 / 10 | **8 / 10** |
| **Performance** | 7 / 10 | 6 / 10 | **7 / 10** |
| **Overall** | **7 / 10** | **6.5 / 10** | **7 / 10** |

---

## 1. SOLID

### Backend — 8 / 10

**✅ Strengths**

- **Single Responsibility** — Each class has one clear job: `ValidationFilter` only validates, `HandlingLogFilter` only logs, `BaseRepository` only manages data access, `PagedResult` only carries paged data.
- **Open/Closed** — The `IModule` interface combined with reflection-based auto-discovery in `CoreComposition` means adding a new domain requires zero changes to existing code. The `ValidationFilter<T>` generic pipeline similarly extends to new request types for free.
- **Liskov Substitution** — `BaseRepository<T>` is correctly substitutable. All `IXxxRepository` contracts are honoured by their implementations without surprises.
- **Dependency Inversion** — All module endpoints depend exclusively on interfaces (`ICategoriesRepository`, `ICustomersRepository`, etc.), never on concrete classes.

**⚠️ Concerns**

- **Interface Segregation** — Repository interfaces (e.g., `ICategoriesRepository`) mix read queries and write commands in a single contract. As modules grow, splitting into `IXxxReadRepository` / `IXxxWriteRepository` would reduce unnecessary coupling.

---

### Frontend — 7 / 10

**✅ Strengths**

- **Single Responsibility** — API hooks (`useCategoryApi`, `useProductApi`, etc.) are separated from UI components. Each component has a single role: page, table, or form.
- **Open/Closed** — Adding a new feature only requires creating a new folder under `features/`; no existing code needs to change.
- **Dependency Inversion** — Components depend on hooks, hooks depend on `fetch`/TanStack Query abstractions — no direct coupling to the API URL inside components.

**⚠️ Concerns**

- **Interface Segregation** — `PagedParams`, `PagedResult`, `Address`, and contact-related types are duplicated across `catalog/types.ts`, `crm/types.ts`, and `purchasing/types.ts`. Each feature imports only its own copy, violating ISP by forcing each to know and redefine a broader shared contract.
- **Single Responsibility (minor)** — `ProductForm` reaches into the Catalog API hook (`useCategoryApi`) to populate a supplier name lookup — a form component pulling cross-domain data directly.

---

## 2. DDD

### Backend — 6 / 10

**✅ Strengths**

- **Bounded contexts are explicit** — Catalog, CRM, Purchasing, Fulfillment, Reporting, SalesOrdering, SalesOrg each map to a dedicated database schema and a self-contained module folder.
- **Correct layering** — Repository interfaces live in `Application/`, implementations in `Infrastructure/`. The domain layer does not reference EF Core directly.
- **Value Objects** — `Address`, `Contact`, and `PhoneNumbers` are shared `record` types used consistently across CRM and Purchasing, providing structural equality and immutability.
- **DTO separation** — `sealed record` DTOs cleanly separate the API surface from the persistence model.

**⚠️ Concerns**

- **No domain entities or aggregates** — `Category`, `Product`, `Customer`, `Supplier` are pure EF data models with no domain behaviour, invariants, or encapsulation. This is acceptable for a CRUD-centric application but means DDD is applied structurally (context boundaries) rather than behaviourally (rich domain model).
- **Context boundary leak** — `CatalogDbContext` maps `SupplierEntity` from the `purchasing` schema solely to perform a join for supplier names on the product list. This is a direct reach across bounded context boundaries. The correct approach is an anti-corruption layer or a dedicated read-model DTO resolved from the Purchasing context.
- **No domain events** — There is no eventing infrastructure. Cross-context reactions (e.g., "when an order is placed, update inventory") have no defined pattern yet.
- **No inter-module contracts** — If SalesOrdering needs to reference a Customer from CRM, the pattern for that is currently undefined.

---

### Frontend — 5 / 10

**✅ Strengths**

- **Feature-sliced structure** mirrors the backend's bounded context layout — one folder per domain.
- Each feature is self-contained: types, API hooks, and UI are co-located.

**⚠️ Concerns**

- **Shared types duplicated** — `PagedResult<T>`, `PagedParams`, `Address`, `Contact`, and `PhoneNumbers` are redefined independently in each feature's `types.ts`. These are shared domain concepts and should live in a `shared/types.ts` or `lib/types.ts` module.
- **No ubiquitous language enforcement** — Type names (`CrmEntry`, `SupplierSummary`, `SupplierDetail`) do not always align with the backend's DDD naming (`CustomerSummaryDto`, `SupplierSummaryDto`). Small drift now, wider divergence later.
- **Stub pages** (`FulfillmentPage`, `ReportingPage`, `SalesOrderingPage`, `SalesOrgPage`) are placeholders with no domain context — acceptable for scaffolding but not DDD-aligned yet.

---

## 3. Clean Architecture

Clean Architecture (Robert C. Martin) defines a strict **dependency rule**: source code dependencies must always point inward — from infrastructure → application → domain — and the inner layers must never know about outer layers. It prescribes four conceptual rings: Entities, Use Cases, Interface Adapters, and Frameworks & Drivers.

### Backend — 6 / 10

**✅ Strengths**

- **Dependency direction is mostly correct** — `Northwind.Core` (inner) never references `Northwind.Api` (outer). The API project depends on Core, not the reverse.
- **Repository interfaces as ports** — `ICategoriesRepository`, `ICustomersRepository`, `ISuppliersRepository` act as Clean Architecture ports. Concrete EF implementations in `Infrastructure/` are the adapters. The application layer depends only on the abstraction.
- **Infrastructure isolation** — `DbContext` classes, entity mappings, and EF-specific code are fully contained in `Infrastructure/` sub-folders. No EF types leak into `Application/` or module-level code.
- **Framework-agnostic inner contracts** — `IValidatable`, `PagedResult<T>`, `IBaseRepository`, and all DTOs carry zero framework dependencies. They could be extracted and tested without ASP.NET Core or EF.
- **`IModule` as an interface adapter** — Each module's `MapEndpoints` acts as the Interface Adapter layer, translating HTTP concerns (routes, status codes, filters) into calls to application-layer repository interfaces.

**⚠️ Concerns**

- **No Use Case layer** — Clean Architecture prescribes explicit Use Case / Interactor classes that encapsulate application business rules independently of delivery mechanism. Here, all application logic (orchestration, mapping, error handling) is inlined as endpoint handler lambdas directly inside each module's `MapEndpoints`. This merges the Interface Adapter and Use Case layers into one, making logic harder to test in isolation and harder to reuse across delivery mechanisms (e.g., a CLI, a message consumer).
- **`IModule` interface leaks framework types** — `IModule.RegisterServices` takes `IServiceCollection` and `MapEndpoints` returns `IEndpointRouteBuilder` — both ASP.NET Core types. A truly clean inner boundary would express module contracts in terms of framework-agnostic abstractions, with ASP.NET specifics confined to the outermost layer.
- **`ValidationFilter<T>` and `HandlingLogFilter` live in `Shared/Abstractions`** — These are ASP.NET Core `IEndpointFilter` implementations (framework-specific). Placing them in `Abstractions/` (conceptually an inner layer) blurs the boundary. They belong in an outer adapter layer.
- **`BaseRepository<T>` depends on `DbContext`** — The abstract base in `Shared/Infrastructure` directly couples to EF's `DbContext`. This is pragmatic and contained, but it means the infrastructure base class cannot be swapped for a non-EF implementation without changing the shared base.
- **`CatalogDbContext` maps `SupplierEntity`** — As noted in DDD, the Catalog infrastructure layer directly maps a Purchasing entity. In Clean Architecture terms, this is an outer-layer (Infrastructure) concern that reaches across module boundaries, violating the principle that adapters should only reference their own module's application layer.
- **No test project** — Clean Architecture's primary motivation is testability of inner layers without infrastructure. The `tests/` folder exists but is empty (`.gitkeep` only). The Use Case / application logic is currently untestable in isolation.

---

### Frontend — 6 / 10

**✅ Strengths**

- **API hooks as adapters** — `useCategoryApi`, `useProductApi`, `useCrmApi`, `useSupplierApi` act as interface adapters: they translate HTTP calls into typed domain objects that UI components consume. Components never call `fetch` directly.
- **Types as entities** — Each feature's `types.ts` defines the domain shape (`CategoryDetails`, `SupplierSummary`, etc.) independently of the API transport layer.
- **TanStack Query as the framework driver** — Components are not coupled to TanStack Query directly; they consume hook return values. Swapping the data-fetching library would only require changing the hooks, not the components.

**⚠️ Concerns**

- **No explicit use case layer** — There is no layer between UI components and API hooks that encapsulates application logic (e.g., "create a supplier and then navigate to its detail page"). This logic is inlined in form `onSubmit` handlers, mixing interface adapter and use case concerns.
- **Shared types duplicated** — `PagedResult<T>`, `Address`, `Contact`, and `PhoneNumbers` are redefined in each feature's `types.ts`. In Clean Architecture terms, these are inner-layer (entity-level) types that should be defined once and shared outward.
- **`ProductForm` crosses feature boundaries** — The product form directly calls `useCategoryApi` from the Catalog feature. This is an outer layer (UI component) bypassing the intended inward-only dependency direction to reach across an adjacent module.
- **No dependency injection** — There is no DI container on the frontend. API hook dependencies (base URL, auth tokens, etc.) are resolved via module-level constants rather than injected interfaces, making unit testing of hooks harder.

---

## 4. Maintainability

### Backend — 7 / 10

**✅ Strengths**

- **Consistent patterns** — Every implemented module (Catalog, CRM, Purchasing) is structurally identical. A developer who reads one module can immediately navigate any other.
- **`file`-scoped route constants** — Route strings are encapsulated inside the module file, invisible to other modules.
- **`sealed record` DTOs** — Immutability and structural equality are baked in; `sealed` prevents unintended inheritance.
- **Primary constructor syntax** throughout — idiomatic modern C#.
- **Contract validation now enforces DB length constraints** — command request validation now checks varchar-aligned limits for names and shared value-object fields (`Contact`, `Address`, `Communication`), returning clean `400` responses instead of relying on DB exceptions.
- **Skeleton modules** for unimplemented domains stub health endpoints and establish the full architecture surface without blocking delivery.

**⚠️ Concerns**

- **No global exception handling** — There is no `UseExceptionHandler` or `IProblemDetailsService`. Unhandled exceptions will leak stack traces in Development and return empty `500`s in Production.
- **`SupplierMappings` is empty dead code** — The file exists with no members and unused `using` directives. It should be removed or completed.
- **Double-logging on health checks** — `BaseRepository.GetCountAsync` logs the count fetch, then the concrete override also logs, producing duplicate log entries per health check call.
- **Request-timing middleware inlined in `Program.cs`** — The stopwatch lambda is harder to test and reuse. It should be extracted to a proper `IMiddleware` or extension method.
- **`SuppliersRepository.AddAsync` ID generation** — Catches broad `Exception` (instead of `PostgresException` with error code `23505`) and retries in a loop. A non-PK constraint violation would cause an infinite loop. `Random` is also instantiated per call instead of using `Random.Shared`.

---

### Frontend — 7 / 10

**✅ Strengths**

- **Consistent feature structure** — Types, hooks, table, form, and page components follow the same layout across all implemented features.
- **TanStack Query** usage is idiomatic: `isPending` / `isError` guards, cache invalidation on mutation, `retry: false` globally.
- **`@/` path alias** configured in both `tsconfig.json` and `vite.config.ts` — clean absolute imports throughout.

**⚠️ Concerns**

- **`Pagination` component is defined but never used** — `components/Pagination.tsx` implements a proper windowed page-number algorithm. However, `CategoriesPage`, `CustomersPage`, and `SuppliersPage` all implement their own inline Previous/Next pagination, ignoring it. Either adopt it consistently or remove it.
- **Inline Tailwind classes duplicated** — Form components repeat the same `className` strings across `CrmForm`, `CategoryForm`, `ProductForm`, and `SupplierForm`. Extracting a shared `<TextField>` or `<FormField>` component would reduce noise.
- **Missing barrel exports** — Only `catalog/` exports via `index.ts`. `crm/` and `purchasing/` import directly from the folder, creating inconsistency.
- **`useHealthCheck` 5-second hardcoded startup delay** — Every navigation item delays its first health-check fetch by 5000ms. This leaves the sidebar counts blank for 5 seconds on every page load. A shorter delay or smarter retry strategy would improve UX.

---

## 5. Scalability

### Backend — 6 / 10

**✅ Strengths**

- **Pagination** is implemented on Products, Customers, and Suppliers.
- **`AsNoTracking()`** is consistently applied on all read queries.
- **Static EF projections** (e.g., `ProductEntity.ToSummary`, `SupplierEntity.ToSummary`) are translated to SQL — no over-fetching of full entity graphs.
- **No N+1 queries** — `Include` / `Join` is used correctly for joined data.

**⚠️ Concerns**

- **Two DB round-trips per paginated query** — All paginated methods issue a `CountAsync` and then the paged query separately. A single query using `COUNT(*) OVER ()` (raw SQL window function) would halve round-trips at scale.
- **Multiple `DbContext` instances per request** — Each module has its own `DbContext` (e.g., `CatalogDbContext`, `CrmDbContext`, `SupplierDbContext`), all registered `Scoped`. A cross-module request creates multiple contexts and opens multiple connections. Npgsql's pool mitigates this, but it is worth monitoring.
- **No caching** — No `IMemoryCache` or `IOutputCache` anywhere. The categories list and health endpoints (static-ish data) are good candidates for short-lived caching.
- **No async streaming** — All queries materialise full result lists into memory. For large datasets, `IAsyncEnumerable` / streaming would reduce peak memory usage.
- **Categories are not paginated** — The full categories list is loaded in one query. Acceptable for a small lookup table, but worth revisiting as data grows.

---

### Frontend — 6 / 10

**✅ Strengths**

- **`keepPreviousData`** (`placeholderData: keepPreviousData`) is used in Products, Customers, and Suppliers — prevents loading flicker on page change.
- **TanStack Query cache** deduplicates simultaneous requests for the same key (e.g., health checks across multiple nav items).

**⚠️ Concerns**

- **8 simultaneous health check queries on initial render** — Each navigation item mounts its own `useHealthCheck`. While TanStack Query deduplicates by key, the architectural design couples navigation rendering to backend liveness, creating 8 parallel network requests on every load.
- **No lazy loading / code splitting** — All feature pages are eagerly imported in `App.tsx`. Using `React.lazy` + `Suspense` would reduce initial bundle size as the application grows.
- **No virtual scrolling** — Tables render all returned rows directly. If page sizes grow, large DOM trees will degrade render performance.
- **`useHealthCheck` 5-second delay** — Covered under Maintainability; also a scalability/UX concern for apps with many modules.

---

## 6. Modularity

### Backend — 8 / 10

**✅ Strengths**

- **Zero inter-module code coupling** (with one exception noted below) — modules share only the `Shared/` contracts.
- **`IModule` auto-discovery** means new modules are registered with no changes to `Program.cs` or `CoreComposition.cs`.
- **`file`-scoped route constant classes** ensure route strings are module-private.
- **Central package management** via `Directory.Packages.props` — only two NuGet packages, no version drift risk.
- **`FrameworkReference` on `Northwind.Core`** instead of explicit ASP.NET Core package references — correct for a class library consumed by a web host.

**⚠️ Concerns**

- **Context boundary leak in `CatalogDbContext`** — `SupplierEntity` is mapped directly in the Catalog module's `DbContext` as a cross-context join target. This is the only modularity violation in the backend.
- **No inter-module contracts defined** — If a module needs data from another, the architectural pattern for that (shared read models, domain events, ACL, etc.) is undefined.

---

### Frontend — 7 / 10

**✅ Strengths**

- Each feature folder is fully self-contained — types, hooks, and components are co-located.
- The `@/` alias enables clean, refactor-safe imports.

**⚠️ Concerns**

- **Shared types are not shared** — `PagedResult<T>`, `PagedParams`, `Address`, `Contact`, and `PhoneNumbers` are duplicated across three feature `types.ts` files instead of living in a `shared/` module.
- **Inconsistent barrel exports** — Only `catalog/index.ts` exists. `crm/` and `purchasing/` lack barrel files, creating an inconsistent import pattern.
- **`ProductForm` imports `useCategoryApi`** from the Catalog feature — a cross-feature dependency not mediated by any shared contract.

---

## 7. Performance

### Backend — 7 / 10

| Area | Finding |
|---|---|
| Read queries | `AsNoTracking()` consistently applied ✅ |
| SQL projections | Static `ToSummary` / `ToDetail` expressions translated to SQL ✅ |
| N+1 | Not present — joins handled correctly ✅ |
| Pagination | Implemented on most list endpoints ✅ |
| Count + data | Two round-trips per paginated call ⚠️ |
| Caching | None ⚠️ |
| Streaming | None ⚠️ |
| ID generation retry | `Random` instantiated per call; over-broad exception catch ⚠️ |

---

### Frontend — 6 / 10

| Area | Finding |
|---|---|
| Pagination | Implemented with `keepPreviousData` ✅ |
| Query deduplication | TanStack Query deduplicates by key ✅ |
| Code splitting | None — all pages eagerly imported ⚠️ |
| Health checks on load | 8 parallel queries on initial render ⚠️ |
| Virtual scrolling | Not implemented ⚠️ |
| 5-second startup delay | Hard-coded delay on every health check ⚠️ |

---

## 8. Key Issues (Priority Order)

| # | Priority | Issue | Location |
|---|---|---|---|
| 1 | 🔴 High | No global exception handling — unhandled exceptions return blank `500`s | `Program.cs` |
| 2 | 🔴 High | Context boundary leak — `SupplierEntity` mapped inside `CatalogDbContext` | `CatalogDbContext.cs` |
| 3 | 🟠 Medium | `SuppliersRepository.AddAsync` catches broad `Exception` in a retry loop; uses `new Random()` per call | `SuppliersRepository.cs` |
| 4 | 🟠 Medium | `Pagination` component defined but never used; pages implement inline pagination | `Pagination.tsx` |
| 5 | 🟠 Medium | Shared types (`PagedResult`, `Address`, `Contact`) duplicated across three feature `types.ts` files | `catalog/`, `crm/`, `purchasing/` |
| 6 | 🟡 Low | `SupplierMappings.cs` is empty dead code | `SupplierMappings.cs` |
| 7 | 🟡 Low | `useHealthCheck` 5-second hard-coded startup delay — poor UX | `useHealthCheck.ts` |
| 8 | 🟡 Low | Double-logging on health check calls | `BaseRepository.cs` |
| 9 | 🟡 Low | Request-timing middleware inlined in `Program.cs` — should be extracted | `Program.cs` |
| 10 | 🟡 Low | Missing barrel `index.ts` for `crm/` and `purchasing/` features | `crm/`, `purchasing/` |
| 11 | 🟡 Low | No code splitting — all pages eagerly imported | `App.tsx` |

---

## 9. What Is Done Well

- **Reflection-based module auto-discovery** is elegant — `Program.cs` and `CoreComposition.cs` never need to change when a new domain is added.
- **`file`-scoped route constants** per module prevent global namespace pollution and route string drift.
- **`IValidatable` + `ValidationFilter<T>`** pipeline is clean, framework-agnostic, and keeps validation logic in request contracts and shared value objects.
- **`sealed record` DTOs** with primary constructors — immutable, structurally comparable, modern C#.
- **Skeleton modules** for unimplemented domains establish the full architecture surface from day one without blocking delivery.
- **EF SQL projections** via static expression fields avoid over-fetching entity graphs.
- **TanStack Query** usage is idiomatic — pending/error guards, cache invalidation on mutation, `retry: false` globally.
- **Vite proxy** (`/api` → `http://localhost:5019`) eliminates CORS configuration during development — clean DX.
- **Nullable-aware codebase** on both sides (`<Nullable>enable</Nullable>` in C#; `strict: true` in TypeScript) — the type system enforces null safety throughout.
