#!/usr/bin/env bash

set -euo pipefail

CONTAINER_NAME="${CONTAINER_NAME:-northwind-db}"
POSTGRES_USER="${POSTGRES_USER:-postgres}"
POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-postgres}"
POSTGRES_DB="${POSTGRES_DB:-northwind}"
HOST_PORT="${HOST_PORT:-5435}"

if ! command -v docker >/dev/null 2>&1; then
  echo "docker is required but was not found in PATH."
  exit 1
fi

if ! command -v python3 >/dev/null 2>&1; then
  echo "python3 is required but was not found in PATH."
  exit 1
fi

if docker ps -a --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
  echo "Container '${CONTAINER_NAME}' already exists. Remove it first:"
  echo "  docker rm -f ${CONTAINER_NAME}"
  exit 1
fi

echo "Starting container '${CONTAINER_NAME}'..."
if ! docker run -d \
  --name "${CONTAINER_NAME}" \
  -e POSTGRES_USER="${POSTGRES_USER}" \
  -e POSTGRES_PASSWORD="${POSTGRES_PASSWORD}" \
  -e POSTGRES_DB="${POSTGRES_DB}" \
  -p "${HOST_PORT}:5432" \
  postgres:17; then
  echo "Failed to start container '${CONTAINER_NAME}'."
  echo "Tip: port ${HOST_PORT} may already be in use."
  exit 1
fi

echo "Waiting for PostgreSQL to be ready..."
until docker exec "${CONTAINER_NAME}" pg_isready -U "${POSTGRES_USER}" -q; do
  if [[ "$(docker inspect -f '{{.State.Running}}' "${CONTAINER_NAME}" 2>/dev/null || true)" != "true" ]]; then
    echo "Container '${CONTAINER_NAME}' is not running."
    echo "Check logs: docker logs ${CONTAINER_NAME}"
    exit 1
  fi
  sleep 1
done

echo "Running init.sql..."
docker exec -i "${CONTAINER_NAME}" psql -v ON_ERROR_STOP=1 -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" < "$(dirname "$0")/init.sql"

echo "Running seed.sql..."
python3 - <<'PY' "$(dirname "$0")/seed.sql" \
  | docker exec -i "${CONTAINER_NAME}" psql -v ON_ERROR_STOP=1 -U "${POSTGRES_USER}" -d "${POSTGRES_DB}"
from pathlib import Path
import re
import sys

seed_path = Path(sys.argv[1])
content = seed_path.read_text(encoding='utf-8', errors='ignore')

# Remove a leftover SQL Server command block embedded after the final Orders row.
content = re.sub(
  r'\)\s+go\s+set\s+identity_insert\s+"Orders"\s+off\s+go\s+ALTER\s+TABLE\s+"Orders"\s+CHECK\s+CONSTRAINT\s+ALL\s+go\s+set\s+quoted_identifier\s+on\s+go\s+set\s+identity_insert\s+"Products"\s+on\s+go\s+ALTER\s+TABLE\s+"Products"\s+NOCHECK\s+CONSTRAINT\s+ALL\s+go;',
  ');',
  content,
  flags=re.IGNORECASE,
)

# Remove trailing sequence setval statements (not required for this setup run).
content = re.sub(r'(?im)^\s*SELECT\s+(?:pg_catalog\.)?setval\(.*$', '', content)

# Add statement terminators before known statement starters when the previous
# non-whitespace character is not already ';'. This safely handles multiline SQL.
boundary_pattern = re.compile(r'\n(?=(INSERT INTO |SET session_replication_role = DEFAULT;|COMMIT;|-- ))')

def add_missing_terminator(match: re.Match[str]) -> str:
  i = match.start() - 1
  while i >= 0 and content[i] in ' \t\r\n':
    i -= 1
  if i >= 0 and content[i] == ';':
    return '\n'
  return ';\n'

content = boundary_pattern.sub(add_missing_terminator, content)

# Rewrite legacy Northwind quoted table names to physical snake_case table names.
insert_table_name_map = {
  '"Employees"': 'employees',
  '"Categories"': 'categories',
  '"Customers"': 'customers',
  '"Shippers"': 'shippers',
  '"Suppliers"': 'suppliers',
  '"Orders"': 'orders',
  '"Products"': 'products',
  '"Order Details"': 'order_lines',
  '"CustomerDemographics"': 'customer_demographic_types',
  '"CustomerCustomerDemo"': 'customer_demographic_assignments',
  '"Region"': 'regions',
  '"Territories"': 'territories',
  '"EmployeeTerritories"': 'employee_territory_assignments',
}

for legacy_name, physical_name in insert_table_name_map.items():
  content = re.sub(
    rf'(?im)\bINSERT\s+INTO\s+{re.escape(legacy_name)}',
    f'INSERT INTO {physical_name}',
    content,
  )

# Rewrite legacy quoted INSERT column lists to snake_case columns.
insert_column_name_map = {
  'employees': {
    '"EmployeeID"': 'employee_id',
    '"LastName"': 'last_name',
    '"FirstName"': 'first_name',
    '"Title"': 'title',
    '"TitleOfCourtesy"': 'courtesy_title',
    '"BirthDate"': 'birth_date',
    '"HireDate"': 'hired_at',
    '"Address"': 'address',
    '"City"': 'city',
    '"Region"': 'region',
    '"PostalCode"': 'postal_code',
    '"Country"': 'country',
    '"HomePhone"': 'home_phone',
    '"Extension"': 'extension',
    '"Photo"': 'photo',
    '"Notes"': 'notes',
    '"ReportsTo"': 'manager_employee_id',
    '"PhotoPath"': 'photo_path',
  },
  'categories': {
    '"CategoryID"': 'category_id',
    '"CategoryName"': 'category_name',
    '"Description"': 'description',
    '"Picture"': 'picture',
  },
  'customers': {
    '"CustomerID"': 'customer_id',
    '"CompanyName"': 'company_name',
    '"ContactName"': 'contact_name',
    '"ContactTitle"': 'contact_title',
    '"Address"': 'address',
    '"City"': 'city',
    '"Region"': 'region',
    '"PostalCode"': 'postal_code',
    '"Country"': 'country',
    '"Phone"': 'phone',
    '"Fax"': 'fax',
  },
  'shippers': {
    '"ShipperID"': 'shipper_id',
    '"CompanyName"': 'company_name',
    '"Phone"': 'phone',
  },
  'suppliers': {
    '"SupplierID"': 'supplier_id',
    '"CompanyName"': 'company_name',
    '"ContactName"': 'contact_name',
    '"ContactTitle"': 'contact_title',
    '"Address"': 'address',
    '"City"': 'city',
    '"Region"': 'region',
    '"PostalCode"': 'postal_code',
    '"Country"': 'country',
    '"Phone"': 'phone',
    '"Fax"': 'fax',
    '"HomePage"': 'homepage_url',
  },
  'orders': {
    '"OrderID"': 'order_id',
    '"CustomerID"': 'customer_id',
    '"EmployeeID"': 'employee_id',
    '"OrderDate"': 'ordered_at',
    '"RequiredDate"': 'required_at',
    '"ShippedDate"': 'shipped_at',
    '"ShipVia"': 'shipper_id',
    '"Freight"': 'freight_amount',
    '"ShipName"': 'ship_name',
    '"ShipAddress"': 'ship_address',
    '"ShipCity"': 'ship_city',
    '"ShipRegion"': 'ship_region',
    '"ShipPostalCode"': 'ship_postal_code',
    '"ShipCountry"': 'ship_country',
  },
  'products': {
    '"ProductID"': 'product_id',
    '"ProductName"': 'product_name',
    '"SupplierID"': 'supplier_id',
    '"CategoryID"': 'category_id',
    '"QuantityPerUnit"': 'quantity_per_unit',
    '"UnitPrice"': 'unit_price',
    '"UnitsInStock"': 'units_in_stock',
    '"UnitsOnOrder"': 'units_on_order',
    '"ReorderLevel"': 'reorder_level',
    '"Discontinued"': 'is_discontinued',
  },
  'order_lines': {
    '"OrderID"': 'order_id',
    '"ProductID"': 'product_id',
    '"UnitPrice"': 'unit_price',
    '"Quantity"': 'quantity',
    '"Discount"': 'discount_rate',
  },
  'customer_demographic_types': {
    '"CustomerTypeID"': 'customer_type_id',
    '"CustomerDesc"': 'customer_description',
  },
  'customer_demographic_assignments': {
    '"CustomerID"': 'customer_id',
    '"CustomerTypeID"': 'customer_type_id',
  },
  'regions': {
    '"RegionID"': 'region_id',
    '"RegionDescription"': 'region_description',
  },
  'territories': {
    '"TerritoryID"': 'territory_id',
    '"TerritoryDescription"': 'territory_description',
    '"RegionID"': 'region_id',
  },
  'employee_territory_assignments': {
    '"EmployeeID"': 'employee_id',
    '"TerritoryID"': 'territory_id',
  },
}

for table_name, columns_map in insert_column_name_map.items():
  pattern = re.compile(
    rf'(?im)^(INSERT\s+INTO\s+{re.escape(table_name)}\s*\()([^\)]*)(\)\s*VALUES\s*\()',
  )

  def rewrite_insert_columns(match: re.Match[str]) -> str:
    columns = [c.strip() for c in match.group(2).split(',')]
    rewritten = [columns_map.get(c, c) for c in columns]
    return f"{match.group(1)}{', '.join(rewritten)}{match.group(3)}"

  content = pattern.sub(rewrite_insert_columns, content)

# PostgreSQL boolean normalization for Products.Discontinued.
content = re.sub(
  r'^(INSERT INTO (?:"Products"|products)\(.*(?:"Discontinued"|is_discontinued)\) VALUES\(.*),(0|1)\);\s*$',
  lambda m: f"{m.group(1)},{'true' if m.group(2) == '1' else 'false'});",
  content,
  flags=re.MULTILINE,
)

sys.stdout.write(content if content.endswith('\n') else content + '\n')
PY

echo "Done. Northwind database is ready at localhost:${HOST_PORT}."
echo "Connection: postgresql://${POSTGRES_USER}:${POSTGRES_PASSWORD}@localhost:${HOST_PORT}/${POSTGRES_DB}"
