import { HealthStatus } from "@/components/HealthStatus";

export function HomePage() {
  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold">Dashboard</h1>
      <HealthStatus endpoint="/api/health" label="API Health" />
    </div>
  );
}
