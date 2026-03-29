import {
  useQuery,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import type {
  ProductSummary,
  ProductDetails,
  ProductRequest,
  PagedResult,
} from "./types";

const PRODUCTS_KEY = "products";
const CATALOG_HEALTH_KEY = ["health", "/api/catalog/health"];

export function useProducts() {
  return useQuery({
    queryKey: [PRODUCTS_KEY],
    queryFn: async (): Promise<PagedResult<ProductSummary>> => {
      const res = await fetch("/api/catalog/products?pageSize=100");
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      return res.json();
    },
  });
}

export function useProduct(productId: number | null) {
  return useQuery({
    queryKey: ["product", productId],
    queryFn: async (): Promise<ProductDetails> => {
      const res = await fetch(`/api/catalog/products/${productId}`);
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
      return res.json();
    },
    enabled: productId !== null,
  });
}

export function useCreateProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: ProductRequest) => {
      const res = await fetch("/api/catalog/products", {
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
      queryClient.invalidateQueries({ queryKey: [PRODUCTS_KEY] });
      queryClient.invalidateQueries({ queryKey: CATALOG_HEALTH_KEY });
    },
  });
}

export function useUpdateProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      productId,
      request,
    }: {
      productId: number;
      request: ProductRequest;
    }) => {
      const res = await fetch(`/api/catalog/products/${productId}`, {
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
      queryClient.invalidateQueries({ queryKey: [PRODUCTS_KEY] });
      queryClient.invalidateQueries({ queryKey: CATALOG_HEALTH_KEY });
    },
  });
}

export function useDiscontinueProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (productId: number) => {
      const res = await fetch(`/api/catalog/products/${productId}/discontinue`, {
        method: "PATCH",
      });
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [PRODUCTS_KEY] });
      queryClient.invalidateQueries({ queryKey: CATALOG_HEALTH_KEY });
    },
  });
}

export function useReinstateProduct() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (productId: number) => {
      const res = await fetch(`/api/catalog/products/${productId}/reinstate`, {
        method: "PATCH",
      });
      if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [PRODUCTS_KEY] });
      queryClient.invalidateQueries({ queryKey: CATALOG_HEALTH_KEY });
    },
  });
}
