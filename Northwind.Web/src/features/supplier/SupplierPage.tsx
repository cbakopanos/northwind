import { useState } from "react";
import { Plus, PackageOpen } from "lucide-react";
import { useSuppliers, useSupplier, useCreateSupplier, useUpdateSupplier } from "./useSupplierApi";
import { SupplierTable, SupplierTableSkeleton } from "./SupplierTable";
import { SupplierForm } from "./SupplierForm";
import type { SupplierRequest } from "./types";

type PanelState =
  | { mode: "closed" }
  | { mode: "add" }
  | { mode: "edit"; supplierId: number };

export function SupplierPage() {
  const [panel, setPanel] = useState<PanelState>({ mode: "closed" });

  const { data: suppliers, isLoading, isError, error } = useSuppliers();

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

      {suppliers && suppliers.length === 0 && (
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

      {suppliers && suppliers.length > 0 && (
        <SupplierTable
          suppliers={suppliers}
          onEdit={(id) => setPanel({ mode: "edit", supplierId: id })}
        />
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
