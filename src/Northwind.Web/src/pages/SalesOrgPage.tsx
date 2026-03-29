import { HealthStatus } from "@/components/HealthStatus";

export function SalesOrgPage() {
  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold">Sales Organization</h1>
      <HealthStatus endpoint="/api/sales-org/health" label="Sales Org Health" />
    </div>
  );
}
