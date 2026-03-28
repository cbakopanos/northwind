# Domain Design Proposal (DDD) for Northwind

## 1) Intro

This document proposes a Domain-Driven Design (DDD) model for the current Northwind relational schema.

The goal is to move from table-centric design to domain-centric design by:

- Splitting the model into clear bounded contexts.
- Defining aggregates and transactional consistency boundaries.
- Making cross-context interactions explicit with a context map.
- Providing concrete aggregate contracts (commands, events, repository boundaries).

### Scope and assumptions

- Source schema: [database/init.sql](../database/init.sql)
- Existing data loading: [database/seed.sql](../database/seed.sql)
- Bootstrap script: [database/rundb.sh](../database/rundb.sh)
- Existing reporting SQL views/functions are treated as read models and not as transactional domain logic.
- Cross-context consistency is eventual unless explicitly stated.

### Domain priorities

1. Keep **Order** behavior central and explicit.
2. Isolate mutable master data (Product, Customer, Employee, Supplier, Shipper) from transactional flows.
3. Prefer references by ID across context boundaries.
4. Use domain events for integration and reporting.

---

## 2) Bounded Contexts

## 2.1 Sales Ordering (Core Domain)

**Purpose**

- Capture customer orders, price/discount snapshots, and order lifecycle.

**Primary tables**

- `Orders`
- `Order Details`

**Key responsibilities**

- Create draft orders.
- Add/remove/change lines.
- Validate line and order invariants.
- Place and manage order state transitions.

**Owns language**

- Order, OrderLine, Draft, Placed, Shipped, Cancelled, Subtotal.

---

## 2.2 Product Catalog

**Purpose**

- Manage products and categories that Sales Ordering references.

**Primary tables**

- `Products`
- `Categories`

**Key responsibilities**

- Product identity and metadata.
- Category structure and assignment.
- Product sellability (`Discontinued`).

**Owns language**

- Product, Category, Discontinued, UnitPrice, ReorderLevel.

---

## 2.3 Customer Management (CRM)

**Purpose**

- Manage customer profile and segmentation.

**Primary tables**

- `Customers`
- `CustomerDemographics`
- `CustomerCustomerDemo`

**Key responsibilities**

- Customer identity and contact profile.
- Customer demographic tagging.

**Owns language**

- Customer, DemographicTag, Contact, Account.

---

## 2.4 Fulfillment & Shipping

**Purpose**

- Manage shipping providers and shipment processing concerns.

**Primary tables**

- `Shippers`
- Shipping fields currently stored on `Orders` (`ShipVia`, `ShippedDate`, destination fields)

**Key responsibilities**

- Carrier definitions and availability.
- Shipping execution state and timestamps.

**Owns language**

- Shipper, Shipment, ShipMethod, ShipDate.

---

## 2.5 Sales Organization

**Purpose**

- Manage internal sales structure and territory assignment.

**Primary tables**

- `Employees`
- `Region`
- `Territories`
- `EmployeeTerritories`

**Key responsibilities**

- Employee profile and manager hierarchy.
- Territory and region ownership.
- Employee-to-territory assignments.

**Owns language**

- Employee, Territory, Region, Manager, Assignment.

---

## 2.6 Supplier Management

**Purpose**

- Manage supplier profile and supplier-product relation metadata.

**Primary tables**

- `Suppliers`
- Product supplier reference via `Products.SupplierID`

**Key responsibilities**

- Supplier commercial profile.
- Supplier contact and operational metadata.

**Owns language**

- Supplier, VendorProfile.

---

## 2.7 Reporting & Analytics (Read Model)

**Purpose**

- Serve analytical views and reporting queries.

**Primary objects**

- Views/functions like `Invoices`, `Order Subtotals`, `Sales by Year`, `Product Sales for 1997`, etc.

**Key responsibilities**

- Provide denormalized, query-optimized read models.
- Subscribe to domain events from transactional contexts.

**Important note**

- This context is **read-only** from a domain perspective.

---

## 3) Aggregates

## 3.1 Sales Ordering aggregates

### Aggregate root: `Order`

**Entities/Value Objects inside aggregate**

- `OrderLine` (entity)
- `ShippingAddress` (value object snapshot)
- `Money` (value object)

**Consistency boundary**

- One order and its lines are transactional together.
- No cross-aggregate transaction with Product, Customer, Employee, or Shipper.

**Invariants**

- Order has at least one line before placement.
- No duplicate product lines within same order.
- Line quantity > 0.
- Line discount between 0 and 1.
- Captured line unit price is immutable after placement.
- State transitions are valid (`Draft -> Placed -> Shipped` or `Draft/Placed -> Cancelled`).

---

## 3.2 Product Catalog aggregates

### Aggregate root: `Product`

**Invariants**

- `UnitPrice >= 0`
- `UnitsInStock >= 0`
- `UnitsOnOrder >= 0`
- `ReorderLevel >= 0`
- If `Discontinued = true`, product cannot be newly added to an order.

### Aggregate root: `Category`

**Invariants**

- Unique category identity.
- Category name required.

---

## 3.3 Customer Management aggregates

### Aggregate root: `Customer`

**Invariants**

- Stable external identity (`CustomerID`).
- Company name required.
- Demographic links must reference existing tags.

### Aggregate root: `CustomerDemographic`

**Invariants**

- Stable tag identity (`CustomerTypeID`).

---

## 3.4 Fulfillment & Shipping aggregates

### Aggregate root: `Shipper`

**Invariants**

- Company name required.

### Aggregate root: `Shipment` (recommended domain abstraction)

**Notes**

- `Shipment` may be persisted through order shipping fields initially.
- Can later move to dedicated shipment storage.

**Invariants**

- Linked to exactly one order.
- Cannot ship a cancelled order.
- `ShippedDate` cannot be before `OrderDate`.

---

## 3.5 Sales Organization aggregates

### Aggregate root: `Employee`

**Invariants**

- Manager (`ReportsTo`) must refer to valid employee when present.
- No self-management cycles.

### Aggregate root: `Territory`

**Invariants**

- Territory belongs to exactly one `Region`.

### Aggregate root: `Region`

**Invariants**

- Stable region identity and description.

---

## 3.6 Supplier Management aggregates

### Aggregate root: `Supplier`

**Invariants**

- Company name required.
- Commercial profile fields are internally consistent.

---

## 3.7 Reporting aggregates

- None (read model context).

---

## 4) Context Map

Visual diagram: [DOMAIN.mmd](DOMAIN.mmd)

## 4.1 High-level relationships

- **Sales Ordering** consumes identities from:
  - Product Catalog (`ProductId`, Category rules)
  - Customer Management (`CustomerId`)
  - Sales Organization (`EmployeeId`)
  - Fulfillment (`ShipperId`)

- **Product Catalog** references Supplier Management (`SupplierId`).

- **Reporting** subscribes to events from all transactional contexts.

## 4.2 Integration style

- In-process domain events (short term).
- Outbox + integration events (medium term).
- Eventual consistency between contexts.
- Anti-Corruption Layer (ACL) for legacy SQL naming/structures (for example, `Order Details`).

## 4.3 Context map pattern labels

- Product Catalog, CRM, Sales Organization, Fulfillment, Supplier: **Upstream** master data providers.
- Sales Ordering: **Downstream** consumer and core process owner.
- Reporting: **Downstream read model** consumer.

## 4.4 Diagram parity (DOMAIN.md <-> DOMAIN.mmd)

The visual map in [DOMAIN.mmd](DOMAIN.mmd) is expected to match these edges exactly:

- `Product Catalog -> Sales Ordering` (`ProductId`, category rules)
- `Customer Management -> Sales Ordering` (`CustomerId`)
- `Sales Organization -> Sales Ordering` (`EmployeeId`)
- `Fulfillment & Shipping -> Sales Ordering` (`ShipperId`)
- `Supplier Management -> Product Catalog` (`SupplierId` reference)
- `Sales Ordering -> Reporting` (`OrderPlaced`, `OrderShipped`, `OrderCancelled`)
- `Product Catalog -> Reporting` (`ProductPriceChanged`, `ProductDiscontinued`)
- `Customer Management -> Reporting` (`Customer*` events)
- `Sales Organization -> Reporting` (`Employee*` events)
- `Supplier Management -> Reporting` (`Supplier*` events)
- `Fulfillment & Shipping -> Reporting` (`Shipper*` / `Shipment*` events)
- ACL dashed links from `Sales Ordering` to upstream master-data contexts (`Product Catalog`, `Customer Management`, `Sales Organization`, `Fulfillment & Shipping`).

---

## 5) Concrete Aggregate Contract Proposals

This section defines command/event/repository boundaries for each context.

## 5.1 Sales Ordering contracts

### `Order` aggregate commands

- `CreateOrder(orderId, customerId, employeeId, orderDate, requiredDate, shippingAddress)`
- `AddOrderLine(orderId, productId, quantity, unitPriceSnapshot, discount)`
- `ChangeOrderLineQuantity(orderId, productId, quantity)`
- `ChangeOrderLineDiscount(orderId, productId, discount)`
- `RemoveOrderLine(orderId, productId)`
- `AssignShipper(orderId, shipperId)`
- `PlaceOrder(orderId)`
- `MarkOrderShipped(orderId, shippedDate)`
- `CancelOrder(orderId, reason)`

### `Order` aggregate events

- `OrderCreated`
- `OrderLineAdded`
- `OrderLineQuantityChanged`
- `OrderLineDiscountChanged`
- `OrderLineRemoved`
- `OrderShipperAssigned`
- `OrderPlaced`
- `OrderShipped`
- `OrderCancelled`

### Repository boundary

- `OrderRepository`
  - `get(orderId): Order`
  - `save(order: Order): void`
  - `exists(orderId): bool`
- Repository stores **only** `Order` aggregate state.
- Product/customer/shipper/employee data loaded through domain services or read models, not foreign aggregate loading.

---

## 5.2 Product Catalog contracts

### `Product` aggregate commands

- `CreateProduct(productId, productName, supplierId, categoryId, unitPrice, quantityPerUnit)`
- `RenameProduct(productId, newName)`
- `ChangeProductPrice(productId, newUnitPrice)`
- `AdjustInventory(productId, deltaInStock)`
- `SetReorderLevel(productId, reorderLevel)`
- `DiscontinueProduct(productId)`
- `ReinstateProduct(productId)`
- `ChangeProductCategory(productId, categoryId)`

### `Product` aggregate events

- `ProductCreated`
- `ProductRenamed`
- `ProductPriceChanged`
- `ProductInventoryAdjusted`
- `ProductReorderLevelChanged`
- `ProductDiscontinued`
- `ProductReinstated`
- `ProductCategoryChanged`

### `Category` aggregate commands/events

- Commands: `CreateCategory`, `RenameCategory`, `DescribeCategory`
- Events: `CategoryCreated`, `CategoryRenamed`, `CategoryDescriptionChanged`

### Repository boundary

- `ProductRepository` (for `Product` aggregate)
- `CategoryRepository` (for `Category` aggregate)
- Do not combine product+category transactions unless explicitly required by aggregate redesign.

---

## 5.3 Customer Management contracts

### `Customer` aggregate commands

- `RegisterCustomer(customerId, companyName, contactInfo, address)`
- `UpdateCustomerContact(customerId, contactInfo)`
- `UpdateCustomerAddress(customerId, address)`
- `AssignDemographic(customerId, customerTypeId)`
- `RemoveDemographic(customerId, customerTypeId)`
- `RenameCustomerCompany(customerId, companyName)`

### `Customer` aggregate events

- `CustomerRegistered`
- `CustomerContactUpdated`
- `CustomerAddressUpdated`
- `CustomerDemographicAssigned`
- `CustomerDemographicRemoved`
- `CustomerCompanyRenamed`

### `CustomerDemographic` commands/events

- Commands: `CreateDemographicTag`, `UpdateDemographicDescription`
- Events: `DemographicTagCreated`, `DemographicDescriptionUpdated`

### Repository boundary

- `CustomerRepository`
- `CustomerDemographicRepository`
- Assignment links (`CustomerCustomerDemo`) are controlled by `Customer` aggregate behavior.

---

## 5.4 Fulfillment & Shipping contracts

### `Shipper` aggregate commands/events

- Commands:
  - `CreateShipper(shipperId, companyName, phone)`
  - `RenameShipper(shipperId, companyName)`
  - `UpdateShipperPhone(shipperId, phone)`
- Events:
  - `ShipperCreated`
  - `ShipperRenamed`
  - `ShipperPhoneUpdated`

### `Shipment` aggregate commands/events (if promoted)

- Commands:
  - `CreateShipment(shipmentId, orderId, shipperId, destination)`
  - `DispatchShipment(shipmentId, shippedDate)`
  - `MarkShipmentDelivered(shipmentId, deliveredDate)`
- Events:
  - `ShipmentCreated`
  - `ShipmentDispatched`
  - `ShipmentDelivered`

### Repository boundary

- `ShipperRepository`
- `ShipmentRepository` (if `Shipment` is explicit)
- If `Shipment` is not explicit yet, `OrderRepository` handles shipping state as part of `Order` aggregate.

---

## 5.5 Sales Organization contracts

### `Employee` aggregate commands/events

- Commands:
  - `HireEmployee(employeeId, profile, hireDate)`
  - `ChangeEmployeeTitle(employeeId, title)`
  - `ChangeEmployeeManager(employeeId, managerId)`
  - `AssignTerritory(employeeId, territoryId)`
  - `UnassignTerritory(employeeId, territoryId)`
- Events:
  - `EmployeeHired`
  - `EmployeeTitleChanged`
  - `EmployeeManagerChanged`
  - `EmployeeTerritoryAssigned`
  - `EmployeeTerritoryUnassigned`

### `Territory` aggregate commands/events

- Commands: `CreateTerritory`, `RenameTerritory`, `MoveTerritoryToRegion`
- Events: `TerritoryCreated`, `TerritoryRenamed`, `TerritoryRegionChanged`

### `Region` aggregate commands/events

- Commands: `CreateRegion`, `RenameRegion`
- Events: `RegionCreated`, `RegionRenamed`

### Repository boundary

- `EmployeeRepository`
- `TerritoryRepository`
- `RegionRepository`
- Territory assignments may be owned by `Employee` if assignment rules are employee-centric.

---

## 5.6 Supplier Management contracts

### `Supplier` aggregate commands

- `RegisterSupplier(supplierId, companyName, contactInfo, address)`
- `UpdateSupplierContact(supplierId, contactInfo)`
- `UpdateSupplierAddress(supplierId, address)`
- `UpdateSupplierHomepage(supplierId, homepage)`

### `Supplier` aggregate events

- `SupplierRegistered`
- `SupplierContactUpdated`
- `SupplierAddressUpdated`
- `SupplierHomepageUpdated`

### Repository boundary

- `SupplierRepository`
- Product linkage remains cross-context (`supplierId` reference in Product Catalog).

---

## 5.7 Reporting & Analytics contracts

### Commands

- None (read-only).

### Events consumed

- `OrderPlaced`, `OrderShipped`, `OrderCancelled`
- `ProductPriceChanged`, `ProductDiscontinued`
- `Customer*` profile events
- `Employee*` assignment events
- `Supplier*` profile events
- `Shipper*` and `Shipment*` events (when Shipment is promoted to explicit aggregate)

### Repository boundary

- Read-model stores only (`ReportingReadStore`, projections).

---

## 6) Implementation notes and migration path

### Step 1: Introduce contracts without schema changes

- Keep existing tables.
- Implement aggregates in application layer.
- Use repository adapters to current table names.

### Step 2: Add event publication

- Publish domain events from successful command handling.
- Build reporting projections from events.

### Step 3: Optional structural improvements

- Introduce explicit `Shipment` persistence.
- Introduce outbox for reliable cross-context event delivery.
- Split physical schemas/databases per bounded context when organizationally justified.

---

## 7) Suggested ubiquitous language glossary

- **Order**: customer purchase request with line snapshots.
- **OrderLine**: product, quantity, unit-price snapshot, discount.
- **Placed Order**: order accepted and immutable except approved transitions.
- **Discontinued Product**: product not allowed in new order lines.
- **Territory Assignment**: relation between sales employee and sales territory.
- **Shipper**: carrier used to deliver order.
- **Shipment**: operational execution of order delivery.

---

## 8) Consistency & completeness check

This pass confirms consistency between schema, scripts, and domain docs.

### 8.1 Schema to context alignment

- `Orders`, `Order Details` -> Sales Ordering
- `Products`, `Categories` -> Product Catalog
- `Customers`, `CustomerDemographics`, `CustomerCustomerDemo` -> Customer Management
- `Shippers` (+ shipping fields in `Orders`) -> Fulfillment & Shipping
- `Employees`, `Region`, `Territories`, `EmployeeTerritories` -> Sales Organization
- `Suppliers` -> Supplier Management
- Views/functions -> Reporting & Analytics read model

### 8.2 Script alignment

- [database/rundb.sh](../database/rundb.sh) provisions PostgreSQL, runs [database/init.sql](../database/init.sql), then [database/seed.sql](../database/seed.sql).
- Script includes preprocessing required to execute converted seed data safely.
- Script now supports environment overrides for `CONTAINER_NAME`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`, and `HOST_PORT`.

### 8.3 Context-map alignment

- [DOMAIN.mmd](DOMAIN.mmd) and section 4 describe the same upstream/downstream relationships.
- Reporting is modeled as downstream read model consuming events from all transactional contexts.

### 8.4 Known intentional gaps

- Physical database split by bounded context is not implemented yet.
- Outbox/integration transport is planned but not implemented.
- `Shipment` persistence is still embedded in order shipping fields unless promoted later.

### 8.5 Schema-to-domain deltas (intentional)

- In [../database/init.sql](../database/init.sql), `Orders.CustomerID`, `Orders.EmployeeID`, and `Orders.ShipVia` are nullable.
  - Domain command contracts may enforce stricter rules for new writes (for example, requiring `customerId`/`employeeId` before `PlaceOrder`).
  - Nullable values remain relevant for historical imports and legacy compatibility.
- Database check constraints in `Products` and `Order Details` are reflected in aggregate invariants, while lifecycle rules (for example, `Draft -> Placed -> Shipped`) remain domain-level behavior.
