# Northwind Database Reference

## Intro

This document describes the current PostgreSQL database structure used in this repository, based on [database/init.sql](../database/init.sql), with data loaded by [database/seed.sql](../database/seed.sql) through [database/rundb.sh](../database/rundb.sh).

It is a practical inventory of database objects (tables, views, functions, indexes, constraints) and a quick navigation aid for development.

Visual ER diagram: [DATABASE.mmd](DATABASE.mmd)

---

## 1) Quick summary

- Engine: PostgreSQL (container image `postgres:17` via [database/rundb.sh](../database/rundb.sh))
- Extension: `pgcrypto`
- Tables: 13
- Views: 16
- SQL functions (procedure equivalents): 7
- Explicit secondary indexes: 26

---

## 2) Tables

## 2.1 Core transactional tables

1. `Orders`
   - PK: `OrderID`
   - FKs:
     - `CustomerID -> Customers.CustomerID`
     - `EmployeeID -> Employees.EmployeeID`
     - `ShipVia -> Shippers.ShipperID`
   - Purpose: order header, lifecycle dates, shipping destination and freight.

2. `Order Details`
   - Composite PK: (`OrderID`, `ProductID`)
   - FKs:
     - `OrderID -> Orders.OrderID`
     - `ProductID -> Products.ProductID`
   - Purpose: line items with unit price snapshot, quantity, and discount.

3. `Products`
   - PK: `ProductID`
   - FKs:
     - `SupplierID -> Suppliers.SupplierID`
     - `CategoryID -> Categories.CategoryID`
   - Purpose: product catalog and inventory/sellability metadata.

## 2.2 Master/reference tables

4. `Customers`
   - PK: `CustomerID`
   - Purpose: customer profile and contact/address information.

5. `Employees`
   - PK: `EmployeeID`
   - Self-FK: `ReportsTo -> Employees.EmployeeID`
   - Purpose: employee profile and manager hierarchy.

6. `Shippers`
   - PK: `ShipperID`
   - Purpose: shipping providers.

7. `Suppliers`
   - PK: `SupplierID`
   - Purpose: supplier profile and contacts.

8. `Categories`
   - PK: `CategoryID`
   - Purpose: product category metadata.

## 2.3 Relationship/association tables

9. `CustomerDemographics`
   - PK: `CustomerTypeID`
   - Purpose: demographic type dictionary.

10. `CustomerCustomerDemo`
   - Composite PK: (`CustomerID`, `CustomerTypeID`)
   - FKs:
     - `CustomerID -> Customers.CustomerID`
     - `CustomerTypeID -> CustomerDemographics.CustomerTypeID`
   - Purpose: many-to-many link between customers and demographic tags.

11. `Region`
   - PK: `RegionID`
   - Purpose: region catalog.

12. `Territories`
   - PK: `TerritoryID`
   - FK: `RegionID -> Region.RegionID`
   - Purpose: territory catalog per region.

13. `EmployeeTerritories`
   - Composite PK: (`EmployeeID`, `TerritoryID`)
   - FKs:
     - `EmployeeID -> Employees.EmployeeID`
     - `TerritoryID -> Territories.TerritoryID`
   - Purpose: many-to-many assignment between employees and territories.

---

## 3) Views

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
  - `Orders` -> `Customers`, `Employees`, `Shippers`
  - `Order Details` -> `Orders`, `Products`
  - `Products` -> `Categories`, `Suppliers`
  - `Territories` -> `Region`
  - `EmployeeTerritories` -> `Employees`, `Territories`
  - `CustomerCustomerDemo` -> `Customers`, `CustomerDemographics`
  - `Employees.ReportsTo` -> `Employees`

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
