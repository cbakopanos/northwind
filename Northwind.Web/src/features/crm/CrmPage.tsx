import { HealthStatus } from "@/components/HealthStatus";

export function CrmPage() {
  return (
    <div>
      <h1 className="mb-6 text-2xl font-bold">CRM</h1>
      <HealthStatus endpoint="/api/crm/health" label="CRM Health" />
    </div>
  );
}
