import { useState } from "react";
import { Plus, PackageOpen } from "lucide-react";
import {
  useProducts,
  useProduct,
  useCreateProduct,
  useUpdateProduct,
  useDiscontinueProduct,
  useReinstateProduct,
} from "./useProductApi";
import { ProductTable, ProductTableSkeleton } from "./ProductTable";
import { ProductForm } from "./ProductForm";
import type { ProductRequest } from "./types";

const PAGE_SIZE_OPTIONS = [10, 20, 50] as const;

type PanelState =
  | { mode: "closed" }
  | { mode: "add" }
  | { mode: "edit"; productId: number };

export function ProductsPage() {
  const [panel, setPanel] = useState<PanelState>({ mode: "closed" });
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState<number>(10);

  const { data, isLoading, isError, error, isPlaceholderData } = useProducts(page, pageSize);

  const editingId = panel.mode === "edit" ? panel.productId : null;
  const { data: editProduct, isLoading: isLoadingDetail } = useProduct(editingId);

  const createMutation = useCreateProduct();
  const updateMutation = useUpdateProduct();
  const discontinueMutation = useDiscontinueProduct();
  const reinstateMutation = useReinstateProduct();

  const activeMutation = panel.mode === "add" ? createMutation : updateMutation;

  const handleSave = (request: ProductRequest) => {
    if (panel.mode === "add") {
      createMutation.mutate(request, {
        onSuccess: () => setPanel({ mode: "closed" }),
      });
    } else if (panel.mode === "edit") {
      updateMutation.mutate(
        { productId: panel.productId, request },
        { onSuccess: () => setPanel({ mode: "closed" }) },
      );
    }
  };

  const products = data?.items ?? [];

  return (
    <div>
      <div className="mb-6 flex justify-end">
        <button
          onClick={() => setPanel({ mode: "add" })}
          className="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
        >
          <Plus className="h-4 w-4" />
          Add Product
        </button>
      </div>

      {isLoading && <ProductTableSkeleton />}

      {isError && (
        <p className="text-sm text-red-600">
          Failed to load products:{" "}
          {error instanceof Error ? error.message : "Unknown error"}
        </p>
      )}

      {data && products.length === 0 && (
        <div className="flex flex-col items-center justify-center rounded-lg border border-dashed py-16 text-center">
          <PackageOpen className="mb-4 h-12 w-12 text-gray-300" />
          <p className="mb-1 text-lg font-medium text-gray-600">
            No products yet
          </p>
          <p className="mb-4 text-sm text-gray-400">
            Get started by adding your first product.
          </p>
          <button
            onClick={() => setPanel({ mode: "add" })}
            className="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            <Plus className="h-4 w-4" />
            Add Product
          </button>
        </div>
      )}

      {data && products.length > 0 && (
        <>
          <ProductTable
            products={products}
            onEdit={(id) => setPanel({ mode: "edit", productId: id })}
            onDiscontinue={(id) => discontinueMutation.mutate(id)}
            onReinstate={(id) => reinstateMutation.mutate(id)}
          />

          <div className="mt-4 flex items-center justify-between text-sm text-gray-600">
            <div className="flex items-center gap-4">
              <span className="text-sm text-gray-500">
                Showing {(data.page - 1) * data.pageSize + 1}–
                {Math.min(data.page * data.pageSize, data.totalCount)} of {data.totalCount} products
              </span>
              <label className="flex items-center gap-1.5">
                <span className="text-gray-500">Show</span>
                <select
                  value={pageSize}
                  onChange={(e) => {
                    setPageSize(Number(e.target.value));
                    setPage(1);
                  }}
                  className="rounded-md border px-2 py-1 text-sm"
                >
                  {PAGE_SIZE_OPTIONS.map((size) => (
                    <option key={size} value={size}>
                      {size}
                    </option>
                  ))}
                </select>
              </label>
            </div>

            <div className="flex gap-2">
              <button
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={data.page === 1}
                className="inline-flex items-center gap-1 rounded-md border px-3 py-1.5 font-medium hover:bg-gray-50 disabled:opacity-40 disabled:pointer-events-none"
              >
                Previous
              </button>
              <button
                onClick={() => {
                  if (!isPlaceholderData && page < data.totalPages) setPage((p) => p + 1);
                }}
                disabled={isPlaceholderData || page >= data.totalPages}
                className="inline-flex items-center gap-1 rounded-md border px-3 py-1.5 font-medium hover:bg-gray-50 disabled:opacity-40 disabled:pointer-events-none"
              >
                Next
              </button>
            </div>
          </div>
        </>
      )}

      {panel.mode !== "closed" && (
        <ProductForm
          product={panel.mode === "edit" ? editProduct ?? null : null}
          isLoading={panel.mode === "edit" && isLoadingDetail}
          isSaving={activeMutation.isPending}
          error={
            activeMutation.isError
              ? activeMutation.error instanceof Error
                ? activeMutation.error.message
                : "Something went wrong"
              : null
          }
          onSave={handleSave}
          onClose={() => setPanel({ mode: "closed" })}
        />
      )}
    </div>
  );
}
