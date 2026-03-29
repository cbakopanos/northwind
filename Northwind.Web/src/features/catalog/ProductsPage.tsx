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

type PanelState =
  | { mode: "closed" }
  | { mode: "add" }
  | { mode: "edit"; productId: number };

export function ProductsPage() {
  const [panel, setPanel] = useState<PanelState>({ mode: "closed" });

  const { data, isLoading, isError, error } = useProducts();

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
      <div className="mb-6 flex items-center justify-between">
        <h2 className="text-lg font-semibold">Products</h2>
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
        <ProductTable
          products={products}
          onEdit={(id) => setPanel({ mode: "edit", productId: id })}
          onDiscontinue={(id) => discontinueMutation.mutate(id)}
          onReinstate={(id) => reinstateMutation.mutate(id)}
        />
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
