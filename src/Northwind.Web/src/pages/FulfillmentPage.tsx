import { HealthStatus } from "@/components/HealthStatus";

export function FulfillmentPage() {
  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold">Fulfillment</h1>
      <HealthStatus endpoint="/api/fulfillment/health" label="Fulfillment Health" />
    </div>
  );
}
