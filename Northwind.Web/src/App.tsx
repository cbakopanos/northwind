import { Routes, Route } from "react-router-dom";
import { Layout } from "@/components/Layout";
import { HomePage } from "@/features/home";
import { CatalogPage } from "@/features/catalog";
import { CrmPage } from "@/features/crm";
import { FulfillmentPage } from "@/features/fulfillment";
import { ReportingPage } from "@/features/reporting";
import { SalesOrderingPage } from "@/features/sales-ordering";
import { SalesOrgPage } from "@/features/sales-org";
import { SupplierPage } from "@/features/supplier";

export function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="catalog" element={<CatalogPage />} />
        <Route path="crm" element={<CrmPage />} />
        <Route path="fulfillment" element={<FulfillmentPage />} />
        <Route path="reporting" element={<ReportingPage />} />
        <Route path="sales-ordering" element={<SalesOrderingPage />} />
        <Route path="sales-org" element={<SalesOrgPage />} />
        <Route path="supplier" element={<SupplierPage />} />
      </Route>
    </Routes>
  );
}
