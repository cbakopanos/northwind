import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";

const STARTUP_DELAY_MS = 5_000;

export function useHealthCheck(endpoint: string) {
  const [ready, setReady] = useState(false);

  useEffect(() => {
    const id = setTimeout(() => setReady(true), STARTUP_DELAY_MS);
    return () => clearTimeout(id);
  }, []);

  return useQuery({
    queryKey: ["health", endpoint],
    queryFn: async () => {
      const res = await fetch(endpoint);
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      return res.json();
    },
    enabled: ready,
    retry: false,
    refetchOnWindowFocus: false,
  });
}
