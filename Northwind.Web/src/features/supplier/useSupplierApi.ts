import {
  useQuery,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import type {
  SupplierSummary,
  SupplierDetails,
  SupplierRequest,
  PagedResult,
} from "./types";

const SUPPLIERS_KEY = "suppliers";
const SUPPLIER_HEALTH_KEY = ["health", "/api/supplier/health"];

export function useSuppliers(page: number = 1, pageSize: number = 10) {
  return useQuery({
    queryKey: [SUPPLIERS_KEY, page, pageSize],
    queryFn: async (): Promise<PagedResult<SupplierSummary>> => {
      const res = await fetch(`/api/supplier/suppliers?page=${page}&pageSize=${pageSize}`);
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      return res.json();
    },
    placeholderData: (prev) => prev,
  });
}

export function useSupplier(supplierId: number | null) {
  return useQuery({
    queryKey: ["supplier", supplierId],
    queryFn: async (): Promise<SupplierDetails> => {
      const res = await fetch(`/api/supplier/suppliers/${supplierId}`);
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      return res.json();
    },
    enabled: supplierId !== null,
  });
}

export function useCreateSupplier() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: SupplierRequest) => {
      const res = await fetch("/api/supplier/suppliers", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request),
      });
      if (!res.ok) {
        const body = await res.json().catch(() => null);
        throw new Error(body?.error ?? `${res.status} ${res.statusText}`);
      }
      return res.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [SUPPLIERS_KEY] });
      queryClient.invalidateQueries({ queryKey: SUPPLIER_HEALTH_KEY });
    },
  });
}

export function useUpdateSupplier() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      supplierId,
      request,
    }: {
      supplierId: number;
      request: SupplierRequest;
    }) => {
      const res = await fetch(`/api/supplier/suppliers/${supplierId}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request),
      });
      if (!res.ok) {
        const body = await res.json().catch(() => null);
        throw new Error(body?.error ?? `${res.status} ${res.statusText}`);
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [SUPPLIERS_KEY] });
      queryClient.invalidateQueries({ queryKey: SUPPLIER_HEALTH_KEY });
    },
  });
}
