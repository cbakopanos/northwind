import { HealthStatus } from "@/components/HealthStatus";

export function CatalogPage() {
  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold">Catalog</h1>
      <HealthStatus endpoint="/api/catalog/health" label="Catalog Health" />
    </div>
  );
}
