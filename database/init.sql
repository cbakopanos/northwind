/*
** Copyright Microsoft, Inc. 1994 - 2000
** All Rights Reserved.
*/


SET NOCOUNT ON
GO


set quoted_identifier on
GO


/* Set DATEFORMAT so that the date strings are interpreted correctly regardless of
   the default DATEFORMAT on the server.
*/
SET DATEFORMAT mdy
GO


-- This script now assumes objects do not already exist.

CREATE SCHEMA catalog
GO

CREATE SCHEMA crm
GO

CREATE SCHEMA sales_ordering
GO

CREATE SCHEMA sales_org
GO

CREATE SCHEMA fulfillment
GO

CREATE SCHEMA supplier
GO


CREATE TABLE sales_org.employees (
	"EmployeeID" "int" IDENTITY (1, 1) NOT NULL ,
	"LastName" nvarchar (20) NOT NULL ,
	"FirstName" nvarchar (10) NOT NULL ,
	"Title" nvarchar (30) NULL ,
	"TitleOfCourtesy" nvarchar (25) NULL ,
	"BirthDate" "datetime" NULL ,
	"HireDate" "datetime" NULL ,
	"Address" nvarchar (60) NULL ,
	"City" nvarchar (15) NULL ,
	"Region" nvarchar (15) NULL ,
	"PostalCode" nvarchar (10) NULL ,
	"Country" nvarchar (15) NULL ,
	"HomePhone" nvarchar (24) NULL ,
	"Extension" nvarchar (4) NULL ,
	"Photo" "image" NULL ,
	"Notes" "ntext" NULL ,
	"ReportsTo" "int" NULL ,
	"PhotoPath" nvarchar (255) NULL ,
	CONSTRAINT "PK_Employees" PRIMARY KEY  CLUSTERED 
	(
		"EmployeeID"
	),
	CONSTRAINT "FK_Employees_Employees" FOREIGN KEY 
	(
		"ReportsTo"
	) REFERENCES sales_org.employees (
		"EmployeeID"
	),
	CONSTRAINT "CK_Birthdate" CHECK (BirthDate < getdate())
)
GO


 CREATE  INDEX "LastName" ON sales_org.employees("LastName")
GO

 CREATE  INDEX "PostalCode" ON sales_org.employees("PostalCode")
GO


CREATE TABLE catalog.categories (
	"CategoryID" "int" IDENTITY (1, 1) NOT NULL ,
	"CategoryName" nvarchar (15) NOT NULL ,
	"Description" "ntext" NULL ,
	"Picture" "image" NULL ,
	CONSTRAINT "PK_Categories" PRIMARY KEY  CLUSTERED 
	(
		"CategoryID"
	)
)
GO

 CREATE  INDEX "CategoryName" ON catalog.categories("CategoryName")
GO


CREATE TABLE crm.customers (
	"CustomerID" nchar (5) NOT NULL ,
	"CompanyName" nvarchar (40) NOT NULL ,
	"ContactName" nvarchar (30) NULL ,
	"ContactTitle" nvarchar (30) NULL ,
	"Address" nvarchar (60) NULL ,
	"City" nvarchar (15) NULL ,
	"Region" nvarchar (15) NULL ,
	"PostalCode" nvarchar (10) NULL ,
	"Country" nvarchar (15) NULL ,
	"Phone" nvarchar (24) NULL ,
	"Fax" nvarchar (24) NULL ,
	CONSTRAINT "PK_Customers" PRIMARY KEY  CLUSTERED 
	(
		"CustomerID"
	)
)
GO

 CREATE  INDEX "City" ON crm.customers("City")
GO

 CREATE  INDEX "CompanyName" ON crm.customers("CompanyName")
GO

 CREATE  INDEX "PostalCode" ON crm.customers("PostalCode")
GO

 CREATE  INDEX "Region" ON crm.customers("Region")
GO


CREATE TABLE fulfillment.shippers (
	"ShipperID" "int" IDENTITY (1, 1) NOT NULL ,
	"CompanyName" nvarchar (40) NOT NULL ,
	"Phone" nvarchar (24) NULL ,
	CONSTRAINT "PK_Shippers" PRIMARY KEY  CLUSTERED 
	(
		"ShipperID"
	)
)
GO

CREATE TABLE supplier.suppliers (
	"SupplierID" "int" IDENTITY (1, 1) NOT NULL ,
	"CompanyName" nvarchar (40) NOT NULL ,
	"ContactName" nvarchar (30) NULL ,
	"ContactTitle" nvarchar (30) NULL ,
	"Address" nvarchar (60) NULL ,
	"City" nvarchar (15) NULL ,
	"Region" nvarchar (15) NULL ,
	"PostalCode" nvarchar (10) NULL ,
	"Country" nvarchar (15) NULL ,
	"Phone" nvarchar (24) NULL ,
	"Fax" nvarchar (24) NULL ,
	"HomePage" "ntext" NULL ,
	CONSTRAINT "PK_Suppliers" PRIMARY KEY  CLUSTERED 
	(
		"SupplierID"
	)
)
GO

 CREATE  INDEX "CompanyName" ON supplier.suppliers("CompanyName")
GO

 CREATE  INDEX "PostalCode" ON supplier.suppliers("PostalCode")
GO


CREATE TABLE sales_ordering.orders (
	"OrderID" "int" IDENTITY (1, 1) NOT NULL ,
	"CustomerID" nchar (5) NULL ,
	"EmployeeID" "int" NULL ,
	"OrderDate" "datetime" NULL ,
	"RequiredDate" "datetime" NULL ,
	"ShippedDate" "datetime" NULL ,
	"ShipVia" "int" NULL ,
	"Freight" "money" NULL CONSTRAINT "DF_Orders_Freight" DEFAULT (0),
	"ShipName" nvarchar (40) NULL ,
	"ShipAddress" nvarchar (60) NULL ,
	"ShipCity" nvarchar (15) NULL ,
	"ShipRegion" nvarchar (15) NULL ,
	"ShipPostalCode" nvarchar (10) NULL ,
	"ShipCountry" nvarchar (15) NULL ,
	CONSTRAINT "PK_Orders" PRIMARY KEY  CLUSTERED 
	(
		"OrderID"
	),
	CONSTRAINT "FK_Orders_Customers" FOREIGN KEY 
	(
		"CustomerID"
	) REFERENCES crm.customers (
		"CustomerID"
	),
	CONSTRAINT "FK_Orders_Employees" FOREIGN KEY 
	(
		"EmployeeID"
	) REFERENCES sales_org.employees (
		"EmployeeID"
	),
	CONSTRAINT "FK_Orders_Shippers" FOREIGN KEY 
	(
		"ShipVia"
	) REFERENCES fulfillment.shippers (
		"ShipperID"
	)
)
GO

 CREATE  INDEX "CustomerID" ON sales_ordering.orders("CustomerID")
GO

 CREATE  INDEX "CustomersOrders" ON sales_ordering.orders("CustomerID")
GO

 CREATE  INDEX "EmployeeID" ON sales_ordering.orders("EmployeeID")
GO

 CREATE  INDEX "EmployeesOrders" ON sales_ordering.orders("EmployeeID")
GO

 CREATE  INDEX "OrderDate" ON sales_ordering.orders("OrderDate")
GO

 CREATE  INDEX "ShippedDate" ON sales_ordering.orders("ShippedDate")
GO

 CREATE  INDEX "ShippersOrders" ON sales_ordering.orders("ShipVia")
GO

 CREATE  INDEX "ShipPostalCode" ON sales_ordering.orders("ShipPostalCode")
GO


CREATE TABLE catalog.products (
	"ProductID" "int" IDENTITY (1, 1) NOT NULL ,
	"ProductName" nvarchar (40) NOT NULL ,
	"SupplierID" "int" NULL ,
	"CategoryID" "int" NULL ,
	"QuantityPerUnit" nvarchar (20) NULL ,
	"UnitPrice" "money" NULL CONSTRAINT "DF_Products_UnitPrice" DEFAULT (0),
	"UnitsInStock" "smallint" NULL CONSTRAINT "DF_Products_UnitsInStock" DEFAULT (0),
	"UnitsOnOrder" "smallint" NULL CONSTRAINT "DF_Products_UnitsOnOrder" DEFAULT (0),
	"ReorderLevel" "smallint" NULL CONSTRAINT "DF_Products_ReorderLevel" DEFAULT (0),
	"Discontinued" "bit" NOT NULL CONSTRAINT "DF_Products_Discontinued" DEFAULT (0),
	CONSTRAINT "PK_Products" PRIMARY KEY  CLUSTERED 
	(
		"ProductID"
	),
	CONSTRAINT "FK_Products_Categories" FOREIGN KEY 
	(
		"CategoryID"
	) REFERENCES catalog.categories (
		"CategoryID"
	),
	CONSTRAINT "FK_Products_Suppliers" FOREIGN KEY 
	(
		"SupplierID"
	) REFERENCES supplier.suppliers (
		"SupplierID"
	),
	CONSTRAINT "CK_Products_UnitPrice" CHECK (UnitPrice >= 0),
	CONSTRAINT "CK_ReorderLevel" CHECK (ReorderLevel >= 0),
	CONSTRAINT "CK_UnitsInStock" CHECK (UnitsInStock >= 0),
	CONSTRAINT "CK_UnitsOnOrder" CHECK (UnitsOnOrder >= 0)
)
GO

 CREATE  INDEX "CategoriesProducts" ON catalog.products("CategoryID")
GO

 CREATE  INDEX "CategoryID" ON catalog.products("CategoryID")
GO

 CREATE  INDEX "ProductName" ON catalog.products("ProductName")
GO

 CREATE  INDEX "SupplierID" ON catalog.products("SupplierID")
GO

 CREATE  INDEX "SuppliersProducts" ON catalog.products("SupplierID")
GO


CREATE TABLE sales_ordering.order_lines (
	"OrderID" "int" NOT NULL ,
	"ProductID" "int" NOT NULL ,
	"UnitPrice" "money" NOT NULL CONSTRAINT "DF_Order_Details_UnitPrice" DEFAULT (0),
	"Quantity" "smallint" NOT NULL CONSTRAINT "DF_Order_Details_Quantity" DEFAULT (1),
	"Discount" "real" NOT NULL CONSTRAINT "DF_Order_Details_Discount" DEFAULT (0),
	CONSTRAINT "PK_Order_Details" PRIMARY KEY  CLUSTERED 
	(
		"OrderID",
		"ProductID"
	),
	CONSTRAINT "FK_Order_Details_Orders" FOREIGN KEY 
	(
		"OrderID"
	) REFERENCES sales_ordering.orders (
		"OrderID"
	),
	CONSTRAINT "FK_Order_Details_Products" FOREIGN KEY 
	(
		"ProductID"
	) REFERENCES catalog.products (
		"ProductID"
	),
	CONSTRAINT "CK_Discount" CHECK (Discount >= 0 and (Discount <= 1)),
	CONSTRAINT "CK_Quantity" CHECK (Quantity > 0),
	CONSTRAINT "CK_UnitPrice" CHECK (UnitPrice >= 0)
)
GO

 CREATE  INDEX "OrderID" ON sales_ordering.order_lines("OrderID")
GO

 CREATE  INDEX "OrdersOrder_Details" ON sales_ordering.order_lines("OrderID")
GO

 CREATE  INDEX "ProductID" ON sales_ordering.order_lines("ProductID")
GO

 CREATE  INDEX "ProductsOrder_Details" ON sales_ordering.order_lines("ProductID")
GO




set quoted_identifier on
GO

set quoted_identifier on
GO

set quoted_identifier on
GO

set quoted_identifier on
GO

set quoted_identifier on
GO

set quoted_identifier on
GO

set quoted_identifier on
GO

set quoted_identifier on
GO



/* The follwing adds tables to the Northwind database */


CREATE TABLE crm.customer_demographic_assignments 
	([CustomerID] nchar (5) NOT NULL,
	[CustomerTypeID] [nchar] (10) NOT NULL
) ON [PRIMARY]
GO


CREATE TABLE crm.customer_demographic_types 
	([CustomerTypeID] [nchar] (10) NOT NULL ,
	[CustomerDesc] [ntext] NULL 
)  ON [PRIMARY]
GO

	
CREATE TABLE sales_org.regions 
	( [RegionID] [int] NOT NULL ,
	[RegionDescription] [nchar] (50) NOT NULL 
) ON [PRIMARY]
GO


CREATE TABLE sales_org.territories 
	([TerritoryID] [nvarchar] (20) NOT NULL ,
	[TerritoryDescription] [nchar] (50) NOT NULL ,
        [RegionID] [int] NOT NULL
) ON [PRIMARY]
GO


CREATE TABLE sales_org.employee_territory_assignments 
	([EmployeeID] [int] NOT NULL,
	[TerritoryID] [nvarchar] (20) NOT NULL 
) ON [PRIMARY]
GO




--  The following adds constraints to the Northwind database
ALTER TABLE crm.customer_demographic_assignments
	ADD CONSTRAINT [PK_CustomerCustomerDemo] PRIMARY KEY  NONCLUSTERED 
	(
		[CustomerID],
		[CustomerTypeID]
	) ON [PRIMARY]
GO
ALTER TABLE crm.customer_demographic_types
	ADD CONSTRAINT [PK_CustomerDemographics] PRIMARY KEY  NONCLUSTERED 
	(
		[CustomerTypeID]
	) ON [PRIMARY]
GO
ALTER TABLE crm.customer_demographic_assignments
	ADD CONSTRAINT [FK_CustomerCustomerDemo] FOREIGN KEY 
	(
		[CustomerTypeID]
	) REFERENCES crm.customer_demographic_types (
		[CustomerTypeID]
	)
GO
ALTER TABLE crm.customer_demographic_assignments
	ADD CONSTRAINT [FK_CustomerCustomerDemo_Customers] FOREIGN KEY
	(
		[CustomerID]
	) REFERENCES crm.customers (
		[CustomerID]
	)
GO
ALTER TABLE sales_org.regions
	ADD CONSTRAINT [PK_Region] PRIMARY KEY  NONCLUSTERED 
	(
		[RegionID]
	)  ON [PRIMARY]
GO
ALTER TABLE sales_org.territories
	ADD CONSTRAINT [PK_Territories] PRIMARY KEY  NONCLUSTERED 
	(
		[TerritoryID]
	)  ON [PRIMARY]
GO
ALTER TABLE sales_org.territories
	ADD CONSTRAINT [FK_Territories_Region] FOREIGN KEY 
	(
		[RegionID]
	) REFERENCES sales_org.regions (
		[RegionID]
	)
GO
ALTER TABLE sales_org.employee_territory_assignments
	ADD CONSTRAINT [PK_EmployeeTerritories] PRIMARY KEY  NONCLUSTERED 
	(
		[EmployeeID],
		[TerritoryID]
	) ON [PRIMARY]
GO
ALTER TABLE sales_org.employee_territory_assignments
	ADD CONSTRAINT [FK_EmployeeTerritories_Employees] FOREIGN KEY 
	(
		[EmployeeID]
	) REFERENCES sales_org.employees (
		[EmployeeID]
	)
GO
ALTER TABLE sales_org.employee_territory_assignments	
	ADD CONSTRAINT [FK_EmployeeTerritories_Territories] FOREIGN KEY 
	(
		[TerritoryID]
	) REFERENCES sales_org.territories (
		[TerritoryID]
	)
