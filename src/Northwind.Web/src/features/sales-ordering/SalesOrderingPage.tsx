import { HealthStatus } from "@/components/HealthStatus";

export function SalesOrderingPage() {
  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold">Sales Ordering</h1>
      <HealthStatus endpoint="/api/sales-ordering/health" label="Sales Ordering Health" />
    </div>
  );
}
