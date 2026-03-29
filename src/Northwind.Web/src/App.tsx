import { Routes, Route } from "react-router-dom";
import { Layout } from "@/components/Layout";
import { HomePage } from "@/pages/HomePage";
import { CatalogPage } from "@/pages/CatalogPage";
import { CrmPage } from "@/pages/CrmPage";
import { FulfillmentPage } from "@/pages/FulfillmentPage";
import { ReportingPage } from "@/pages/ReportingPage";
import { SalesOrderingPage } from "@/pages/SalesOrderingPage";
import { SalesOrgPage } from "@/pages/SalesOrgPage";
import { SupplierPage } from "@/pages/SupplierPage";

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
