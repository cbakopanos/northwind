import { HealthStatus } from "@/components/HealthStatus";

export function SupplierPage() {
  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold">Supplier</h1>
      <HealthStatus endpoint="/api/supplier/health" label="Supplier Health" />
    </div>
  );
}
