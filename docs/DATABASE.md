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

- `sales_ordering`: `Orders`, `Order Details`
- `catalog`: `Products`, `Categories`
- `crm`: `Customers`, `CustomerDemographics`, `CustomerCustomerDemo`
- `fulfillment`: `Shippers`
- `sales_org`: `Employees`, `Region`, `Territories`, `EmployeeTerritories`
- `supplier`: `Suppliers`

## 2.1 Core transactional tables

1. `sales_ordering."Orders"`
   - PK: `OrderID`
   - FKs:
       - `CustomerID -> crm."Customers"."CustomerID"`
       - `EmployeeID -> sales_org."Employees"."EmployeeID"`
       - `ShipVia -> fulfillment."Shippers"."ShipperID"`
   - Purpose: order header, lifecycle dates, shipping destination and freight.

2. `sales_ordering."Order Details"`
   - Composite PK: (`OrderID`, `ProductID`)
   - FKs:
       - `OrderID -> sales_ordering."Orders"."OrderID"`
       - `ProductID -> catalog."Products"."ProductID"`
   - Purpose: line items with unit price snapshot, quantity, and discount.

3. `catalog."Products"`
   - PK: `ProductID`
   - FKs:
       - `SupplierID -> supplier."Suppliers"."SupplierID"`
       - `CategoryID -> catalog."Categories"."CategoryID"`
   - Purpose: product catalog and inventory/sellability metadata.

## 2.2 Master/reference tables

4. `crm."Customers"`
   - PK: `CustomerID`
   - Purpose: customer profile and contact/address information.

5. `sales_org."Employees"`
   - PK: `EmployeeID`
   - Self-FK: `ReportsTo -> sales_org."Employees"."EmployeeID"`
   - Purpose: employee profile and manager hierarchy.

6. `fulfillment."Shippers"`
   - PK: `ShipperID`
   - Purpose: shipping providers.

7. `supplier."Suppliers"`
   - PK: `SupplierID`
   - Purpose: supplier profile and contacts.

8. `catalog."Categories"`
   - PK: `CategoryID`
   - Purpose: product category metadata.

## 2.3 Relationship/association tables

9. `crm."CustomerDemographics"`
   - PK: `CustomerTypeID`
   - Purpose: demographic type dictionary.

10. `crm."CustomerCustomerDemo"`
   - Composite PK: (`CustomerID`, `CustomerTypeID`)
   - FKs:
   - `CustomerID -> crm."Customers"."CustomerID"`
   - `CustomerTypeID -> crm."CustomerDemographics"."CustomerTypeID"`
   - Purpose: many-to-many link between customers and demographic tags.

11. `sales_org."Region"`
   - PK: `RegionID`
   - Purpose: region catalog.

12. `sales_org."Territories"`
   - PK: `TerritoryID`
   - FK: `RegionID -> sales_org."Region"."RegionID"`
   - Purpose: territory catalog per region.

13. `sales_org."EmployeeTerritories"`
   - Composite PK: (`EmployeeID`, `TerritoryID`)
   - FKs:
   - `EmployeeID -> sales_org."Employees"."EmployeeID"`
   - `TerritoryID -> sales_org."Territories"."TerritoryID"`
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

## 5.2 Categories

- `CategoryName` on (`CategoryName`)

## 5.3 Customers

- `City` on (`City`)
- `CompanyName` on (`CompanyName`)
- `PostalCode_Customers` on (`PostalCode`)
- `Region_Customers` on (`Region`)

## 5.4 Suppliers

- `CompanyName_Suppliers` on (`CompanyName`)
- `PostalCode_Suppliers` on (`PostalCode`)

## 5.5 Orders

- `CustomerID_Orders` on (`CustomerID`)
- `CustomersOrders` on (`CustomerID`)
- `EmployeeID_Orders` on (`EmployeeID`)
- `EmployeesOrders` on (`EmployeeID`)
- `OrderDate` on (`OrderDate`)
- `ShippedDate` on (`ShippedDate`)
- `ShipPostalCode` on (`ShipPostalCode`)
- `ShippersOrders` on (`ShipVia`)

## 5.6 Products

- `CategoryID_Products` on (`CategoryID`)
- `CategoriesProducts` on (`CategoryID`)
- `ProductName_Products` on (`ProductName`)
- `SupplierID_Products` on (`SupplierID`)
- `SuppliersProducts` on (`SupplierID`)

## 5.7 Order Details

- `OrderID_OrderDetails` on (`OrderID`)
- `OrdersOrder_Details` on (`OrderID`)
- `ProductID_OrderDetails` on (`ProductID`)
- `ProductsOrder_Details` on (`ProductID`)

Note: some indexes are intentionally duplicated from original Northwind naming/compatibility conventions.

---

## 6) Constraints and data integrity highlights

### Primary keys

- Every table has a PK; three are composite: `Order Details`, `CustomerCustomerDemo`, `EmployeeTerritories`.

### Foreign keys

- Key referential chains:
   - `sales_ordering."Orders"` -> `crm."Customers"`, `sales_org."Employees"`, `fulfillment."Shippers"`
   - `sales_ordering."Order Details"` -> `sales_ordering."Orders"`, `catalog."Products"`
   - `catalog."Products"` -> `catalog."Categories"`, `supplier."Suppliers"`
   - `sales_org."Territories"` -> `sales_org."Region"`
   - `sales_org."EmployeeTerritories"` -> `sales_org."Employees"`, `sales_org."Territories"`
   - `crm."CustomerCustomerDemo"` -> `crm."Customers"`, `crm."CustomerDemographics"`
   - `sales_org."Employees"."ReportsTo"` -> `sales_org."Employees"`

### Check constraints

- `Employees`: `BirthDate < CURRENT_TIMESTAMP`
- `Products`: non-negative `UnitPrice`, `ReorderLevel`, `UnitsInStock`, `UnitsOnOrder`
- `Order Details`:
  - `Discount` in `[0, 1]`
  - `Quantity > 0`
  - `UnitPrice >= 0`

---

## 7) Naming and migration notes

- The schema keeps original Northwind-style quoted identifiers, including spaced names such as `Order Details`.
- Physical partitioning by bounded context is implemented using schemas in one database.
- Current safety mode keeps both intra-context and cross-context foreign keys active.
- `database/seed.sql` relies on `search_path` to load legacy unqualified inserts into the correct schemas.
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
