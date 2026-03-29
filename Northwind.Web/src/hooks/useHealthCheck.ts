import { useQuery } from "@tanstack/react-query";

export function useHealthCheck(endpoint: string) {
  return useQuery({
    queryKey: ["health", endpoint],
    queryFn: async () => {
      const res = await fetch(endpoint);
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      return res.json();
    },
  });
}
