import { HealthStatus } from "@/components/HealthStatus";

export function ReportingPage() {
  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold">Reporting</h1>
      <HealthStatus endpoint="/api/reporting/health" label="Reporting Health" />
    </div>
  );
}
