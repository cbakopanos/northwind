import {
  useQuery,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import type {
  CategorySummary,
  CategoryDetails,
  CategoryRequest,
} from "./types";

const CATEGORIES_KEY = "categories";
const CATALOG_HEALTH_KEY = ["health", "/api/catalog/health"];

export function useCategories() {
  return useQuery({
    queryKey: [CATEGORIES_KEY],
    queryFn: async (): Promise<CategorySummary[]> => {
      const res = await fetch("/api/catalog/categories");
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      return res.json();
    },
  });
}

export function useCategory(categoryId: number | null) {
  return useQuery({
    queryKey: ["category", categoryId],
    queryFn: async (): Promise<CategoryDetails> => {
      const res = await fetch(`/api/catalog/categories/${categoryId}`);
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      return res.json();
    },
    enabled: categoryId !== null,
  });
}

export function useCreateCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: CategoryRequest) => {
      const res = await fetch("/api/catalog/categories", {
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
      queryClient.invalidateQueries({ queryKey: [CATEGORIES_KEY] });
      queryClient.invalidateQueries({ queryKey: CATALOG_HEALTH_KEY });
    },
  });
}

export function useUpdateCategory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      categoryId,
      request,
    }: {
      categoryId: number;
      request: CategoryRequest;
    }) => {
      const res = await fetch(`/api/catalog/categories/${categoryId}`, {
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
      queryClient.invalidateQueries({ queryKey: [CATEGORIES_KEY] });
      queryClient.invalidateQueries({ queryKey: CATALOG_HEALTH_KEY });
    },
  });
}
