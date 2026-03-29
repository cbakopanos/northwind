import { useHealthCheck } from "@/hooks/useHealthCheck";

interface HealthStatusProps {
  endpoint: string;
  label: string;
}

export function HealthStatus({ endpoint, label }: HealthStatusProps) {
  const { data, isLoading, isError, error } = useHealthCheck(endpoint);

  return (
    <div className="rounded-lg border p-6">
      <h2 className="mb-4 text-lg font-semibold">{label}</h2>
      {isLoading && (
        <p className="text-sm text-gray-500">Checking health…</p>
      )}
      {isError && (
        <p className="text-sm text-red-600">
          Error: {error instanceof Error ? error.message : "Unknown error"}
        </p>
      )}
      {data && (
        <pre className="rounded bg-gray-50 p-3 text-sm text-gray-800">
          {JSON.stringify(data, null, 2)}
        </pre>
      )}
    </div>
  );
}
