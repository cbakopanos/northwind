import { useState } from "react";
import { Plus, PackageOpen, ChevronLeft, ChevronRight } from "lucide-react";
import { useSuppliers, useSupplier, useCreateSupplier, useUpdateSupplier } from "./useSupplierApi";
import { SupplierTable, SupplierTableSkeleton } from "./SupplierTable";
import { SupplierForm } from "./SupplierForm";
import type { SupplierRequest } from "./types";

type PanelState =
  | { mode: "closed" }
  | { mode: "add" }
  | { mode: "edit"; supplierId: number };

const PAGE_SIZE_OPTIONS = [10, 20, 50] as const;

export function SupplierPage() {
  const [panel, setPanel] = useState<PanelState>({ mode: "closed" });
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState<number>(10);

  const { data, isLoading, isError, error, isPlaceholderData } = useSuppliers(page, pageSize);

  const editingId = panel.mode === "edit" ? panel.supplierId : null;
  const { data: editSupplier, isLoading: isLoadingDetail } = useSupplier(editingId);

  const createMutation = useCreateSupplier();
  const updateMutation = useUpdateSupplier();

  const activeMutation = panel.mode === "add" ? createMutation : updateMutation;

  const handleSave = (request: SupplierRequest) => {
    if (panel.mode === "add") {
      createMutation.mutate(request, {
        onSuccess: () => setPanel({ mode: "closed" }),
      });
    } else if (panel.mode === "edit") {
      updateMutation.mutate(
        { supplierId: panel.supplierId, request },
        { onSuccess: () => setPanel({ mode: "closed" }) },
      );
    }
  };

  const suppliers = data?.items ?? [];
  const totalPages = data?.totalPages ?? 1;

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-bold">Suppliers</h1>
        <button
          onClick={() => setPanel({ mode: "add" })}
          className="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
        >
          <Plus className="h-4 w-4" />
          Add Supplier
        </button>
      </div>

      {isLoading && <SupplierTableSkeleton />}

      {isError && (
        <p className="text-sm text-red-600">
          Failed to load suppliers:{" "}
          {error instanceof Error ? error.message : "Unknown error"}
        </p>
      )}

      {data && suppliers.length === 0 && (
        <div className="flex flex-col items-center justify-center rounded-lg border border-dashed py-16 text-center">
          <PackageOpen className="mb-4 h-12 w-12 text-gray-300" />
          <p className="mb-1 text-lg font-medium text-gray-600">
            No suppliers yet
          </p>
          <p className="mb-4 text-sm text-gray-400">
            Get started by adding your first supplier.
          </p>
          <button
            onClick={() => setPanel({ mode: "add" })}
            className="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            <Plus className="h-4 w-4" />
            Add Supplier
          </button>
        </div>
      )}

      {data && suppliers.length > 0 && (
        <>
          <SupplierTable
            suppliers={suppliers}
            onEdit={(id) => setPanel({ mode: "edit", supplierId: id })}
          />

          {(totalPages > 1 || pageSize !== 10) && (
            <div className="mt-4 flex items-center justify-between text-sm text-gray-600">
              <div className="flex items-center gap-4">
                <span>
                  Page {data.page} of {totalPages} · {suppliers.length} suppliers
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
                  disabled={page === 1}
                  className="inline-flex items-center gap-1 rounded-md border px-3 py-1.5 font-medium hover:bg-gray-50 disabled:opacity-40 disabled:pointer-events-none"
                >
                  <ChevronLeft className="h-4 w-4" />
                  Previous
                </button>
                <button
                  onClick={() => {
                    if (!isPlaceholderData && page < totalPages) setPage((p) => p + 1);
                  }}
                  disabled={isPlaceholderData || page >= totalPages}
                  className="inline-flex items-center gap-1 rounded-md border px-3 py-1.5 font-medium hover:bg-gray-50 disabled:opacity-40 disabled:pointer-events-none"
                >
                  Next
                  <ChevronRight className="h-4 w-4" />
                </button>
              </div>
            </div>
          )}
        </>
      )}

      {panel.mode !== "closed" && (
        <SupplierForm
          supplier={panel.mode === "edit" ? editSupplier ?? null : null}
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
