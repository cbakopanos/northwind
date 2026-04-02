import { Routes, Route, Navigate } from "react-router-dom";
import { Layout } from "@/components/Layout";
import { HomePage } from "@/features/home";
import { CatalogPage, CategoriesPage, ProductsPage } from "@/features/catalog";
import { CrmPage, CustomersPage } from "@/features/crm";
import { FulfillmentPage } from "@/features/fulfillment";
import { ReportingPage } from "@/features/reporting";
import { SalesOrderingPage } from "@/features/sales-ordering";
import { SalesOrgPage } from "@/features/sales-org";
import { SupplierPage, SuppliersPage } from "@/features/purchasing";

export function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="catalog" element={<CatalogPage />}>
          <Route index element={<Navigate to="products" replace />} />
          <Route path="categories" element={<CategoriesPage />} />
          <Route path="products" element={<ProductsPage />} />
        </Route>
        <Route path="crm" element={<CrmPage />}>
          <Route index element={<Navigate to="customers" replace />} />
          <Route path="customers" element={<CustomersPage />} />
        </Route>
        <Route path="fulfillment" element={<FulfillmentPage />} />
        <Route path="reporting" element={<ReportingPage />} />
        <Route path="sales-ordering" element={<SalesOrderingPage />} />
        <Route path="sales-org" element={<SalesOrgPage />} />
        <Route path="purchasing" element={<SupplierPage />}>
          <Route index element={<Navigate to="suppliers" replace />} />
          <Route path="suppliers" element={<SuppliersPage />} />
        </Route>
      </Route>
    </Routes>
  );
}
