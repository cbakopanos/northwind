# Northwind Database Reference

## Intro

This document describes the current PostgreSQL database structure used in this repository, based on [database/init.sql](../database/init.sql), with data loaded by [database/seed.sql](../database/seed.sql) through [database/rundb.sh](../database/rundb.sh).

It is a practical inventory of database objects (tables, views, functions, indexes, constraints) and a quick navigation aid for development.

Visual ER diagram: [DATABASE.mmd](DATABASE.mmd)

---

## 1) Quick summary

- Engine: PostgreSQL (container image `postgres:17` via [database/rundb.sh](../database/rundb.sh))
- Extension: `pgcrypto`
- Bounded-context schemas: 7 (`sales_ordering`, `catalog`, `crm`, `fulfillment`, `sales_org`, `supplier`, `reporting`)
- Tables: 13
- Views: 16
- SQL functions (procedure equivalents): 7
- Explicit secondary indexes: 26

---

## 2) Tables

## 2.0 Schema layout

- `sales_ordering`: `orders`, `order_lines`
- `catalog`: `products`, `categories`
- `crm`: `customers`, `customer_demographic_types`, `customer_demographic_assignments`
- `fulfillment`: `shippers`
- `sales_org`: `employees`, `regions`, `territories`, `employee_territory_assignments`
- `supplier`: `suppliers`

## 2.1 Core transactional tables

1. `sales_ordering.orders`
   - PK: `order_id`
   - FKs:
      - `customer_id -> crm.customers.customer_id`
      - `employee_id -> sales_org.employees.employee_id`
      - `shipper_id -> fulfillment.shippers.shipper_id`
   - Purpose: order header, lifecycle dates, shipping destination and freight.

2. `sales_ordering.order_lines`
   - Composite PK: (`order_id`, `product_id`)
   - FKs:
      - `order_id -> sales_ordering.orders.order_id`
      - `product_id -> catalog.products.product_id`
   - Purpose: line items with unit price snapshot, quantity, and discount.

3. `catalog.products`
   - PK: `product_id`
   - FKs:
      - `supplier_id -> supplier.suppliers.supplier_id`
      - `category_id -> catalog.categories.category_id`
   - Purpose: product catalog and inventory/sellability metadata.

## 2.2 Master/reference tables

4. `crm.customers`
   - PK: `customer_id`
   - Purpose: customer profile and contact/address information.

5. `sales_org.employees`
   - PK: `employee_id`
   - Self-FK: `manager_employee_id -> sales_org.employees.employee_id`
   - Purpose: employee profile and manager hierarchy.

6. `fulfillment.shippers`
   - PK: `shipper_id`
   - Purpose: shipping providers.

7. `supplier.suppliers`
   - PK: `supplier_id`
   - Purpose: supplier profile and contacts.

8. `catalog.categories`
   - PK: `category_id`
   - Purpose: product category metadata.

## 2.3 Relationship/association tables

9. `crm.customer_demographic_types`
   - PK: `customer_type_id`
   - Purpose: demographic type dictionary.

10. `crm.customer_demographic_assignments`
   - Composite PK: (`customer_id`, `customer_type_id`)
   - FKs:
   - `customer_id -> crm.customers.customer_id`
   - `customer_type_id -> crm.customer_demographic_types.customer_type_id`
   - Purpose: many-to-many link between customers and demographic tags.

11. `sales_org.regions`
   - PK: `region_id`
   - Purpose: region catalog.

12. `sales_org.territories`
   - PK: `territory_id`
   - FK: `region_id -> sales_org.regions.region_id`
   - Purpose: territory catalog per region.

13. `sales_org.employee_territory_assignments`
   - Composite PK: (`employee_id`, `territory_id`)
   - FKs:
   - `employee_id -> sales_org.employees.employee_id`
   - `territory_id -> sales_org.territories.territory_id`
   - Purpose: many-to-many assignment between employees and territories.

---

## 3) Views

All views are created in schema `reporting` (with `search_path` set to include transactional schemas).

1. `Customer and Suppliers by City`
2. `Alphabetical list of products`
3. `Current Product List`
4. `Orders Qry`
5. `Products Above Average Price`
6. `Products by Category`
7. `Quarterly Orders`
8. `Invoices`
9. `Order Details Extended`
10. `Order Subtotals`
11. `Product Sales for 1997`
12. `Category Sales for 1997`
13. `Sales by Category`
14. `Sales Totals by Amount`
15. `Summary of Sales by Quarter`
16. `Summary of Sales by Year`

These are read-model style SQL objects, mostly oriented to reporting and denormalized querying.

---

## 4) Functions (stored procedure equivalents)

All functions are created in schema `reporting` (with `search_path` set to include transactional schemas).

1. `CustOrdersDetail(p_OrderID integer)`
2. `CustOrdersOrders(p_CustomerID char)`
3. `CustOrderHist(p_CustomerID char)`
4. `SalesByCategory(p_CategoryName varchar, p_OrdYear varchar default '1998')`
5. `Ten Most Expensive Products()`
6. `Employee Sales by Country(p_Beginning_Date timestamp, p_Ending_Date timestamp)`
7. `Sales by Year(p_Beginning_Date timestamp, p_Ending_Date timestamp)`

---

## 5) Indexes

## 5.1 Employees

- `LastName` on (`LastName`)
- `PostalCode` on (`PostalCode`)

Physical indexed columns: `last_name`, `postal_code`

## 5.2 Categories

- `CategoryName` on (`CategoryName`)

Physical indexed column: `category_name`

## 5.3 Customers

- `City` on (`City`)
- `CompanyName` on (`CompanyName`)
- `PostalCode_Customers` on (`PostalCode`)
- `Region_Customers` on (`Region`)

Physical indexed columns: `city`, `company_name`, `postal_code`, `region`

## 5.4 Suppliers

- `CompanyName_Suppliers` on (`CompanyName`)
- `PostalCode_Suppliers` on (`PostalCode`)

Physical indexed columns: `company_name`, `postal_code`

## 5.5 Orders

- `CustomerID_Orders` on (`customer_id`)
- `CustomersOrders` on (`customer_id`)
- `EmployeeID_Orders` on (`employee_id`)
- `EmployeesOrders` on (`employee_id`)
- `OrderDate` on (`ordered_at`)
- `ShippedDate` on (`shipped_at`)
- `ShipPostalCode` on (`ship_postal_code`)
- `ShippersOrders` on (`shipper_id`)

## 5.6 Products

- `CategoryID_Products` on (`category_id`)
- `CategoriesProducts` on (`category_id`)
- `ProductName_Products` on (`product_name`)
- `SupplierID_Products` on (`supplier_id`)
- `SuppliersProducts` on (`supplier_id`)

## 5.7 Order Details

- `OrderID_OrderDetails` on (`order_id`)
- `OrdersOrder_Details` on (`order_id`)
- `ProductID_OrderDetails` on (`product_id`)
- `ProductsOrder_Details` on (`product_id`)

Note: some indexes are intentionally duplicated from original Northwind naming/compatibility conventions.

---

## 6) Constraints and data integrity highlights

### Primary keys

- Every table has a PK; three are composite: `order_lines`, `customer_demographic_assignments`, `employee_territory_assignments`.

### Foreign keys

- Key referential chains:
   - `sales_ordering.orders` -> `crm.customers`, `sales_org.employees`, `fulfillment.shippers`
   - `sales_ordering.order_lines` -> `sales_ordering.orders`, `catalog.products`
   - `catalog.products` -> `catalog.categories`, `supplier.suppliers`
   - `sales_org.territories` -> `sales_org.regions`
   - `sales_org.employee_territory_assignments` -> `sales_org.employees`, `sales_org.territories`
   - `crm.customer_demographic_assignments` -> `crm.customers`, `crm.customer_demographic_types`
   - `sales_org.employees.manager_employee_id` -> `sales_org.employees.employee_id`

### Check constraints

- `employees`: `birth_date < CURRENT_TIMESTAMP`
- `products`: non-negative `unit_price`, `reorder_level`, `units_in_stock`, `units_on_order`
- `order_lines`:
   - `discount_rate` in `[0, 1]`
   - `quantity > 0`
   - `unit_price >= 0`

---

## 7) Naming and migration notes

> **Convention note:** Physical database object names follow `snake_case` for tables, columns, and sequences. Legacy Northwind names are preserved only as reporting output aliases where needed for compatibility.

- Physical partitioning by bounded context is implemented using schemas in one database.
- Physical table names are standardized to unquoted snake_case.
- Current safety mode keeps both intra-context and cross-context foreign keys active.
- Physical column names are also standardized to unquoted snake_case.
- `database/seed.sql` remains in legacy naming format; [database/rundb.sh](../database/rundb.sh) rewrites legacy `INSERT INTO` table and column names to physical snake_case names during bootstrap.
- Functions are implemented via `CREATE OR REPLACE FUNCTION` (instead of SQL Server procedures).
- [database/rundb.sh](../database/rundb.sh) includes seed preprocessing to normalize legacy script fragments before execution.

---

## 8) Operational entry points

- Start and load DB: [database/rundb.sh](../database/rundb.sh)
- Schema DDL: [database/init.sql](../database/init.sql)
- Seed data: [database/seed.sql](../database/seed.sql)
- Domain design: [DOMAIN.md](DOMAIN.md)
- Visual context map: [DOMAIN.mmd](DOMAIN.mmd)
- Visual ER diagram: [DATABASE.mmd](DATABASE.mmd)
