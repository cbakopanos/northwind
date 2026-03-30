import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type { CustomerSummary, CustomerDetails, CustomerRequest, PagedResult } from "./types";

const CUSTOMERS_KEY = "customers";
const CRM_HEALTH_KEY = ["health", "/api/crm/health"];

export function useCustomers(page: number = 1, pageSize: number = 20) {
  return useQuery({
    queryKey: [CUSTOMERS_KEY, page, pageSize],
    queryFn: async (): Promise<PagedResult<CustomerSummary>> => {
      const res = await fetch(`/api/crm/customers?page=${page}&pageSize=${pageSize}`);
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      return res.json();
    },
    placeholderData: (prev) => prev,
  });
}

export function useCustomer(customerId: string | null) {
  return useQuery({
    queryKey: ["customer", customerId],
    queryFn: async (): Promise<CustomerDetails> => {
      const res = await fetch(`/api/crm/customers/${customerId}`);
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      return res.json();
    },
    enabled: customerId !== null,
  });
}

export function useCreateCustomer() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: CustomerRequest) => {
      const res = await fetch("/api/crm/customers", {
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
      queryClient.invalidateQueries({ queryKey: [CUSTOMERS_KEY] });
      queryClient.invalidateQueries({ queryKey: CRM_HEALTH_KEY });
    },
  });
}

export function useUpdateCustomer() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ customerId, request }: { customerId: string; request: CustomerRequest }) => {
      const res = await fetch(`/api/crm/customers/${customerId}`, {
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
      queryClient.invalidateQueries({ queryKey: [CUSTOMERS_KEY] });
      queryClient.invalidateQueries({ queryKey: CRM_HEALTH_KEY });
    },
  });
}
